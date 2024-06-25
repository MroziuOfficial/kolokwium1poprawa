using System.ComponentModel.DataAnnotations;

namespace kolokwium1poprawa.DTOs;

public class RentalDTO
{
    [MaxLength(17)]
    public string Vin { get; set; }
    
    [MaxLength(100)]
    public string Color { get; set; }
    
    [MaxLength(100)]
    public string Model { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalPrice { get; set; }
}