using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
    
    Task<List<ClientTripDTO>> GetTripsForClient(int clientId);
    
    Task<int> AddClient(ClientDTO client);

    Task<ClientDTO> GetClientById(int clientId);

    Task<TripDTO> GetTripById(int tripId);

    Task<int> GetTripParticipantsCount(int tripId);

    Task<bool> RegisterClientForTrip(int clientId, int tripId);
    
    Task<bool> DeleteClientFromTrip(int clientId, int tripId);



}