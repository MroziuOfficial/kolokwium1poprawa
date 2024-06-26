﻿using kolokwium1poprawa.DTOs;

namespace kolokwium1poprawa.Repositories;

public interface ICarRepository
{
    Task<ClientDTO> GetClient(int id);
    Task AddNewClient(NewRentalDTO newClient);
    Task<bool> DoesCarExist(int carId);
    Task<bool> DoesClientExist(int carId);
}