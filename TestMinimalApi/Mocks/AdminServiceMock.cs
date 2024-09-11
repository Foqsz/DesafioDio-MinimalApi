using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinimalApi.Mocks;

public class AdminServiceMock : IAdminService
{
    private static List<Admin> admins = new List<Admin>()
    {
        new Admin()
        {
            Id = 1,
            Email = "adm@adm.com",
            Senha = "123456",
            Perfil = "Adm"
        }, 
        new Admin()
        {
            Id = 2,
            Email = "editor@editor.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };   

    public async Task<IEnumerable<Admin>> GetAdminAll(int? pagina)
    {
        return admins;
    }

    public async Task<Admin>? GetAdminById(int id)
    {
        return admins.Find(a => a.Id == id);
    }

    public async Task<Admin> GetCreateAdmin(Admin admin)
    {
        admin.Id = admins.Count() + 1;
        admins.Add(admin);

        return admin;
    }

    public async Task<Admin> Login(LoginDTO loginDTO)
    {
        return admins.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }
}
