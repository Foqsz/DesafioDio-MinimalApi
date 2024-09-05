using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DesafioDio_MinimalApi.Project.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //garantindo que seja um identity
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nome { get; set; }

        [Required]
        [StringLength(20)]
        public string? Marca { get; set; }

        [Required]
        public int Ano { get; set; }

    }
}
