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
}