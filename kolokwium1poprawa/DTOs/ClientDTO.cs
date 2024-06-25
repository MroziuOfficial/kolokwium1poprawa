using System.ComponentModel.DataAnnotations;

namespace kolokwium1poprawa.DTOs;

public class ClientDTO
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    [MaxLength(100)]
    public string Address { get; set; }
    public List<RentalDTO> Rentals { get; set; } = new List<RentalDTO>();
}