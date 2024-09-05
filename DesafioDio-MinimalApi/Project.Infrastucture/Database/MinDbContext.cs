using DesafioDio_MinimalApi.Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioDio_MinimalApi.Project.Infrastucture.Database;

public class MinDbContext : DbContext
{
    public MinDbContext(DbContextOptions<MinDbContext> options) : base(options)
    { }

    public DbSet<Admin>? Administradores { get; set; }

}
