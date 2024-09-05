using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;

namespace DesafioDio_MinimalApi.Project.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);

}
