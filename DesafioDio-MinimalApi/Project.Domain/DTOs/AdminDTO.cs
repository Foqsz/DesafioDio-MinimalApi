using DesafioDio_MinimalApi.Project.Domain.Enuns;

namespace DesafioDio_MinimalApi.Project.Domain.DTOs;

public class AdminDTO
{
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public Perfil Perfil { get; set; }
}
