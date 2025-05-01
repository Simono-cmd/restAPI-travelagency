using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly ITripsService _tripsService;

        // Konstruktor kontrolera, przyjmuje serwis obsługujący operacje na wycieczkach
        public Controller(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        // 1. GET /api/trips
        // Zwraca listę wszystkich dostępnych wycieczek
        [HttpGet("trips")]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }
        

        // 2. GET /api/clients/{id}/trips
        // Zwraca wycieczki, na które zarejestrowany jest klient o podanym ID
        [HttpGet("clients/{clientId}/trips")]
        public async Task<IActionResult> GetTrip(int clientId)
        {
            var trips = await _tripsService.GetTripsForClient(clientId);

            if (trips == null)
            {
                return NotFound($"Client with ID {clientId} does not exist.");
            }
            if (!trips.Any())
            {
                return NotFound("Client is not registered to any trips.");
            }

            return Ok(trips);
        }


        // 3. POST /api/clients
        // Dodaje nowego klienta do bazy danych po walidacji danych wejściowych
        [HttpPost("clients")]
        public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
        {
            if (string.IsNullOrEmpty(client.FirstName) || string.IsNullOrEmpty(client.LastName) ||
                string.IsNullOrEmpty(client.Email) || string.IsNullOrEmpty(client.Telephone) ||
                string.IsNullOrEmpty(client.Pesel))
            {
                return BadRequest("All fields are required.");
            }

            if (!client.Email.Contains("@"))
            {
                return BadRequest("Invalid email format.");
            }

            if (client.Pesel.Length != 11 || !client.Pesel.All(char.IsDigit))
            {
                return BadRequest("Invalid PESEL.");
            }

            var result = await _tripsService.AddClient(client);
            if (result == 0)  
            {
                return BadRequest("Failed to add client.");
            }
            return Ok(new
            {
                message = "Client added successfully.",
                clientId = result
            });
        }

        // 4. PUT /api/clients/{id}/trips/{tripId}
        // Rejestruje klienta do wycieczki, jeśli nie jest pełna
        [HttpPut("clients/{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientToTrip(int id, int tripId)
        {
            var client = await _tripsService.GetClientById(id);
            if (client == null)
            {
                return NotFound($"Client with ID {id} not found.");
            }

            var trip = await _tripsService.GetTripById(tripId);
            if (trip == null)
            {
                return NotFound($"Trip with ID {tripId} not found.");
            }

            int currentParticipants = await _tripsService.GetTripParticipantsCount(tripId);
            if (currentParticipants >= trip.MaxPeople)
            {
                return BadRequest("The trip is already full.");
            }

            bool success = await _tripsService.RegisterClientForTrip(id, tripId);
            if (success)
            {
                return Ok("Client successfully registered to trip.");
            }
            return StatusCode(500, "Failed to register client to trip.");
            
        }

        // 5. DELETE /api/clients/{id}/trips/{tripId}
        // Usuwa rejestrację klienta z wycieczki
        [HttpDelete("clients/{idClient}/trips/{idTrip}")]
        public async Task<IActionResult> DeleteClientFromTrip(int idClient, int idTrip)
        { bool success = await _tripsService.DeleteClientFromTrip(idClient, idTrip);

            if (!success)
            {
                return StatusCode(500, "Failed to delete client registration");
            }
            return Ok(new { message = $"Client {idClient} successfully deleted from trip {idTrip}" });
        }

    }
}