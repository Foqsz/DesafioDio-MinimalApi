using DesafioDio_MinimalApi.Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioDio_MinimalApi.Project.Infrastucture.Database;

public class MinDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public MinDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Admin>? Administradores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

            var stringDeConexao = _configuration.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(stringDeConexao))
            {
                optionsBuilder.UseMySql(stringDeConexao, ServerVersion.AutoDetect(stringDeConexao));
            }
        }
    }
}
