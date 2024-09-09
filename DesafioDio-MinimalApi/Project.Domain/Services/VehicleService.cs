using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DesafioDio_MinimalApi.Project.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly MinDbContext _context;

    public VehicleService(MinDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehicle>> GetVehicles(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var vehicleQuery = _context.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            vehicleQuery.Where(v => v.Nome.ToLower().Contains(nome));

        }

        int itensPorPagina = 10;

        if (pagina != null)
        {
            vehicleQuery = vehicleQuery.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        return await vehicleQuery.ToListAsync();
    }

    public async Task<Vehicle> GetVehicleId(int id)
    {
        return await _context.Veiculos.Where(v => v.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Vehicle> GetVehicleCreate(Vehicle vehicle)
    {
        var vehicleCreate = _context.Veiculos.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicleCreate.Entity;
    }
    public async Task<bool> GetVehicleUpdate(Vehicle vehicle)
    {
        _context.Veiculos.Update(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> GetVehicleDelete(Vehicle vehicle)
    {
        var vehicleDeleted = _context.Veiculos.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
}
