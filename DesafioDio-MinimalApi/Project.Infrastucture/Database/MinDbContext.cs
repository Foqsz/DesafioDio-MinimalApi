using DesafioDio_MinimalApi.Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioDio_MinimalApi.Project.Infrastucture.Database;

public class MinDbContext : DbContext
{
    public MinDbContext(DbContextOptions<MinDbContext> options) : base(options)
    { }

    public DbSet<Admin> Administradores { get; set; }
    public DbSet<Vehicle> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = 1,
                Email = "admin@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            });
    }
}
