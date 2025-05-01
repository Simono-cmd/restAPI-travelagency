using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = """
                         SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName
                         FROM Trip t
                         LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
                         LEFT JOIN Country c ON ct.IdCountry = c.IdCountry
                         ORDER BY t.IdTrip
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    var trip = new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        Countries = new List<CountryDTO>()
                        
                    };
                    trips.Add(trip);
                    
                    var countryName = reader.IsDBNull(reader.GetOrdinal("CountryName")) ? null : reader.GetString(reader.GetOrdinal("CountryName"));
                    if (!string.IsNullOrEmpty(countryName))
                    {
                        trip.Countries.Add(new CountryDTO { Name = countryName });
                    }
                    
                }
            }
        }
        

        return trips;
    }
    
    public async Task<List<ClientTripDTO>> GetTripsForClient(int clientId)
    {
        var result = new List<ClientTripDTO>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var checkCmd = new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @id", conn);
        checkCmd.Parameters.AddWithValue("@id", clientId);
        var exists = await checkCmd.ExecuteScalarAsync();
        if (exists == null) return null;

        var cmd = new SqlCommand(@"
    SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
           ct.RegisteredAt, ct.PaymentDate
    FROM Trip t
    INNER JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
    WHERE ct.IdClient = @id", conn);

        cmd.Parameters.AddWithValue("@id", clientId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int idOrdinal = reader.GetOrdinal("IdTrip");

            result.Add(new ClientTripDTO
            {
                IdTrip = reader.GetInt32(idOrdinal),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                RegisteredAt = reader.IsDBNull(reader.GetOrdinal("RegisteredAt")) ? 0 : reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentDate")),

            });
        }

        return result;
    }


    public async Task<int> AddClient(ClientDTO client)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        //nie mogą się powtarzać pesel mail i numer telefonu
        var cmd = new SqlCommand(@"
        IF NOT EXISTS (
                SELECT 1 FROM Client
                WHERE Pesel = @Pesel OR Email = @Email OR Telephone = @Telephone
        )
        BEGIN
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            OUTPUT INSERTED.IdClient
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)
        END
        ", conn);


            cmd.Parameters.AddWithValue("@firstName", client.FirstName);
            cmd.Parameters.AddWithValue("@lastName", client.LastName);
            cmd.Parameters.AddWithValue("@email", client.Email);
            cmd.Parameters.AddWithValue("@telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@pesel", client.Pesel);

            var result = await cmd.ExecuteScalarAsync();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
    }
    
    public async Task<ClientDTO> GetClientById(int clientId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", conn);
        cmd.Parameters.AddWithValue("@IdClient", clientId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new ClientDTO
            {
                IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Telephone = reader.GetString(reader.GetOrdinal("Telephone")),
                Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
            };
        }

        return null;
    }

    public async Task<TripDTO> GetTripById(int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Trip WHERE IdTrip = @IdTrip", conn);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new TripDTO
            {
                Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                Countries = new List<CountryDTO>() 
            };
        }

        return null;
    }
    
    public async Task<int> GetTripParticipantsCount(int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @IdTrip", conn);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);

        return (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> RegisterClientForTrip(int clientId, int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        DateTime registeredAt = DateTime.Now;
        string formattedDate = registeredAt.ToString("yyyyMMdd");
        int intDate = Convert.ToInt32(formattedDate);

        var cmd = new SqlCommand(@"
        IF NOT EXISTS (SELECT 1 FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient)
        BEGIN
        INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) 
        VALUES (@IdClient, @IdTrip, @RegisteredAt)
        END
        ELSE
BEGIN
SELECT 0
END
    ", conn);
    
        cmd.Parameters.AddWithValue("@IdClient", clientId);
        cmd.Parameters.AddWithValue("@IdTrip", tripId);
        cmd.Parameters.AddWithValue("@RegisteredAt", intDate);

        var result = await cmd.ExecuteScalarAsync();
        return result == null;
    }

    public async Task<bool> DeleteClientFromTrip(int clientId, int tripId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var checkCmd = new SqlCommand(@"
        SELECT 1
        FROM Client_Trip
        WHERE IdClient = @clientId AND IdTrip = @tripId", conn);
        checkCmd.Parameters.AddWithValue("@clientId", clientId);
        checkCmd.Parameters.AddWithValue("@tripId", tripId);

        var exists = await checkCmd.ExecuteScalarAsync();
        if (exists == null)
        {
            return false; 
        }

        var deleteCmd = new SqlCommand(
            @"
        Delete from Client_Trip where IdClient = @clientId and IdTrip = @tripId
", conn
        );
        deleteCmd.Parameters.AddWithValue("@clientId", clientId);
        deleteCmd.Parameters.AddWithValue("@tripId", tripId);
        
        var rowsAffected = await deleteCmd.ExecuteNonQueryAsync(); // Używamy ExecuteNonQueryAsync(), ponieważ DELETE nie zwraca danych
        return rowsAffected > 0;
    }
    

    
    
    
    
}