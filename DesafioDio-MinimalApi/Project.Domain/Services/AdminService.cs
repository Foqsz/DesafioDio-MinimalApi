using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;

namespace DesafioDio_MinimalApi.Project.Domain.Services;

public class AdminService : IAdminService
{
    private readonly MinDbContext _context;
    public AdminService(MinDbContext dbcontext)
    {
        _context = dbcontext;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm = _context.Administradores.Where(admin => admin.Email == loginDTO.Email && admin.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
         
    }
}
