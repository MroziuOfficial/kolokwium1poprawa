using kolokwium1poprawa.DTOs;
using Microsoft.Data.SqlClient;

namespace kolokwium1poprawa.Repositories;

public class CarRepository : ICarRepository
{
    private readonly IConfiguration _configuration;

    public CarRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesCarExist(int id)
    {
        var query = "SELECT 1 FROM cars WHERE ID = @ID";
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res != null;
    }

    public async Task<bool> DoesClientExist(int id)
    {
        var query = "SELECT 1 FROM clients WHERE ID = @ID";
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res != null;
    }

    public async Task<ClientDTO> GetClient(int id)
    {
        var query = @"
                SELECT 
                    c.ID AS ClientID, c.FirstName, c.LastName, c.Address,
                    car.VIN, co.Name AS Color, m.Name AS Model, r.DateFrom, r.DateTo, r.TotalPrice
                FROM Clients c
                LEFT JOIN Car_Rentals r ON c.ID = r.ClientID
                LEFT JOIN Cars car ON r.CarID = car.ID
                LEFT JOIN Colors co ON car.ColorID = co.ID
                LEFT JOIN Models m ON car.ModelID = m.ID
                WHERE c.ID = @ID";

        var clientDTO = new ClientDTO();
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (clientDTO.Id == 0)
            {
                clientDTO.Id = reader.GetInt32(reader.GetOrdinal("ClientID"));
                clientDTO.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                clientDTO.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                clientDTO.Address = reader.GetString(reader.GetOrdinal("Address"));
            }

            if (!reader.IsDBNull(reader.GetOrdinal("VIN")))
            {
                var rentalDTO = new RentalDTO
                {
                    Vin = reader.GetString(reader.GetOrdinal("VIN")),
                    Color = reader.GetString(reader.GetOrdinal("Color")),
                    Model = reader.GetString(reader.GetOrdinal("Model")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                };
                clientDTO.Rentals.Add(rentalDTO);
            }
        }

        return clientDTO;
    }

    public async Task AddNewClient(NewRentalDTO newClient)
    {
        var totalDays = (newClient.DateTo - newClient.DateFrom).Days;
        var carPriceQuery = "SELECT PricePerDay FROM Cars WHERE ID = @CarID";
        
        await using var conn = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var carPriceCommand = new SqlCommand();
        carPriceCommand.Connection = conn;
        carPriceCommand.CommandText = carPriceQuery;
        carPriceCommand.Parameters.AddWithValue("@CarID", newClient.CarId);
        await conn.OpenAsync();

        var carPrice = (int)await carPriceCommand.ExecuteScalarAsync();
        var totalPrice = carPrice * totalDays;
         
         
        var query = @"INSERT INTO clients (FirstName, LastName, Address) 
                          VALUES(@FirstName, @LastName, @Address);
                          SELECT CAST(SCOPE_IDENTITY() as int);";
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@FirstName", newClient.Client.FirstName);
        command.Parameters.AddWithValue("@LastName", newClient.Client.LastName);
        command.Parameters.AddWithValue("@Address", newClient.Client.Address);

        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            var clientId = (int)await command.ExecuteScalarAsync();

            var rentalQuery =
                "INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice) VALUES(@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice)";
            using (var rentalCommand = new SqlCommand(rentalQuery, connection, transaction as SqlTransaction))
            {
                rentalCommand.Parameters.AddWithValue("@ClientID", clientId);
                rentalCommand.Parameters.AddWithValue("@CarID", newClient.CarId);
                rentalCommand.Parameters.AddWithValue("@DateFrom", newClient.DateFrom);
                rentalCommand.Parameters.AddWithValue("@DateTo", newClient.DateTo);
                rentalCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);
                await rentalCommand.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}