using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;

namespace DesafioDio_MinimalApi.Project.Domain.Services;

public class AdminService : IAdminService
{
    private readonly MinDbContext _context;
    public AdminService(MinDbContext dbcontext)
    {
        _context = dbcontext;
    }

    public async Task<IEnumerable<Admin>> GetAdminAll(int? pagina)
    {
        var vehicleQuery = _context.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            vehicleQuery = vehicleQuery.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return await vehicleQuery.ToListAsync();
    }

    public async Task<Admin> GetAdminById(int id)
    {
        return await _context.Administradores.Where(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Admin> GetCreateAdmin(Admin admin)
    {
        _context.Administradores.Add(admin);
        await _context.SaveChangesAsync();

        return admin;
    }

    public async Task<Admin?> Login(LoginDTO loginDTO)
    {
        var adm = await _context.Administradores.Where(admin => admin.Email == loginDTO.Email && admin.Senha == loginDTO.Senha).FirstOrDefaultAsync();
        return adm;
    }
}
