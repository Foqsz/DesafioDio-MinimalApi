using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioDio_MinimalApi.Project.Domain.Entities;

public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //garantindo que seja um identity
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string? Email { get; set; } 

    [StringLength(20)]
    public string? Senha { get; set; }

    [StringLength(20)]
    public string? Perfil { get; set; }
}
