using EntityFramework.DTOs;
using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EntityFramework.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TripController: ControllerBase
{
    private readonly S28052Context _context;

    public TripController(S28052Context context)
    {
        _context = context;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetTripAsync()
    {
        var trip = await _context.Trips
            .Include(e => e.IdCountries)
            .Include(e => e.ClientTrips)
            .ThenInclude(sg => sg.IdClientNavigation)
            .Select(s => new TripDTO
            {
                Name = s.Name,
                Description = s.Description,
                DateFrom = s.DateFrom,
                DateTo = s.DateTo,
                MaxPeople = s.MaxPeople,
                IdCountries = s.IdCountries.Select(e => new CountryDTO
                {
                    Name = e.Name
                }),
                Client = s.ClientTrips.Select(e => new ClientDTO
                {
                    FirstName = e.IdClientNavigation.FirstName,
                    LastName = e.IdClientNavigation.LastName
                })
            })
            .ToListAsync();
        return Ok(trip);
    }
    
    [HttpDelete("/clients/{idClient}")]
    public async Task<IActionResult> DeleteClientAsync(int idClient)
    {
        var hasTrips = await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == idClient);

        if (hasTrips) return Conflict("Client has trips");

        var clientToDelete = await _context.Clients.FindAsync(idClient);
        if (clientToDelete == null) return NotFound("Client not found");

        await _context.Clients
            .Where(e => e.IdClient == idClient)
            .ExecuteDeleteAsync();

        return Ok();
    }
    
    [HttpPost("trips/{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTripAsync(int idTrip, ClientToTripDTO clientTripDto)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip is null) return NotFound("Trip not found");

        var client = await _context.Clients
            .Where(e => e.Pesel == clientTripDto.Pesel)
            .FirstAsync();
        if (client is null)
        {
            var lastClient = await _context.Clients.OrderByDescending(e=>e.IdClient).FirstAsync();
            await _context.Clients.AddAsync(new Client
            {
                IdClient = lastClient.IdClient,
                FirstName = clientTripDto.FirstName,
                LastName = clientTripDto.LastName,
                Email = clientTripDto.Email,
                Telephone = clientTripDto.Telephone,
                Pesel = clientTripDto.Pesel
            });
        }
        var newClientTrip = new ClientTrip()
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Today,
            PaymentDate = clientTripDto.PaymentDate
        };
        
        await _context.ClientTrips.AddAsync(newClientTrip);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}