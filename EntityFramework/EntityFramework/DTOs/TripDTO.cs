using System.ComponentModel.DataAnnotations;
using EntityFramework.Models;

namespace EntityFramework.DTOs;

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public virtual IEnumerable<ClientDTO> Client { get; set; } = new List<ClientDTO>();

    public virtual IEnumerable<CountryDTO> IdCountries { get; set; } = new List<CountryDTO>();

}

public class ClientDTO
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }

}

public class ClientToTripDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
    public DateTime? PaymentDate { get; set; }
}
