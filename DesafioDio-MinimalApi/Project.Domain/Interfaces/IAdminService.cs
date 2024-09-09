using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;

namespace DesafioDio_MinimalApi.Project.Domain.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<Admin>> GetAdminAll(int? pagina);
    Task<Admin>? GetAdminById(int id);
    Task<Admin> Login(LoginDTO loginDTO);
    Task<Admin> GetCreateAdmin(Admin admin);


}
