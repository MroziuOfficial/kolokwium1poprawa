namespace kolokwium1poprawa.DTOs;

public class NewRentalDTO
{
    public NewClientDTO Client { get; set; }
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}