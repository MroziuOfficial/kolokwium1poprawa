using kolokwium1poprawa.DTOs;
using kolokwium1poprawa.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium1poprawa.Controllers;

[Route("api/clients")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarRepository _carRepository;
    public CarController(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }
        
    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        if (!await _carRepository.DoesClientExist(id))
            return NotFound("The client with specified ID does not exist");
        var client = await _carRepository.GetClient(id);
        return Ok(client);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] NewRentalDTO newClient)
    {
        if (newClient.DateFrom > newClient.DateTo)
            return BadRequest("DateFrom cannot be later than DateTo");
        
        if (!await _carRepository.DoesCarExist(newClient.CarId))
            return NotFound("The car does not exist");
        
        await _carRepository.AddNewClient(newClient);
        return Created("Successfully added: ", newClient);
    }
}