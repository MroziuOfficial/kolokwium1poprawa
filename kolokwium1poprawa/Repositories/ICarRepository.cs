using kolokwium1poprawa.DTOs;

namespace kolokwium1poprawa.Repositories;

public interface ICarRepository
{
    Task<ClientDTO> GetClient(int id);


    //Task AddNewAnimal(NewAnimalDTO newAnimal);
}