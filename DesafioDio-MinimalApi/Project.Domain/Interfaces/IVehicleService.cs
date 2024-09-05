using DesafioDio_MinimalApi.Project.Domain.Entities;

namespace DesafioDio_MinimalApi.Project.Domain.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<Vehicle>> GetVehicles(int pagina = 1, string? nome = null, string? marca = null);
    Task<Vehicle> GetVehicleId(int id);
    Task<Vehicle> GetVehicleCreate(Vehicle vehicle);
    Task<bool> GetVehicleUpdate(Vehicle vehicle);
    Task<bool> GetVehicleDelete(Vehicle vehicle);
}
