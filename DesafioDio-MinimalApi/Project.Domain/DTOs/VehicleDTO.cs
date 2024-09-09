using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DesafioDio_MinimalApi.Project.Domain.DTOs;

public record VehicleDTO
{   
    public string? Nome { get; set; }
     
    public string? Marca { get; set; }
     
    public int Ano { get; set; }
}
