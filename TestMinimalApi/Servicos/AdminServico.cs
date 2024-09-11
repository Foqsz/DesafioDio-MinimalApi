using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Services;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinimalApi.Servicos;

[TestClass]
public class AdminServicoTest
{ 

    private MinDbContext CriarContextDeTeste()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(@"C:\Users\vinic\Documents\Projetos Estudo\DesafioDio-MinimalApi\TestMinimalApi\") 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();


        var configuration = builder.Build();

        var connectString = configuration.GetConnectionString("MySql");

        var options = new DbContextOptionsBuilder<MinDbContext>()
            .UseMySql(connectString, ServerVersion.AutoDetect(connectString)).Options;

        return new MinDbContext(options);
    }

    [TestMethod]
    public async Task TestandoSalvarAdmin()
    {
        // Arrange
        var context = CriarContextDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Admin();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm"; 
        var administradorServico = new AdminService(context);

        // Act
        await administradorServico.GetCreateAdmin(adm);
        var admins = await administradorServico.GetAdminAll(1);

        // Assert
        Assert.AreEqual(1, admins.Count());  
    }

    [TestMethod]
    public async Task BuscarAdminPorId()
    {
        // Arrange
        var context = CriarContextDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Admin();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        var administradorServico = new AdminService(context);

        // Act
        await administradorServico.GetCreateAdmin(adm);
        var admId = await administradorServico.GetAdminById(adm.Id); 

        // Assert
        Assert.AreEqual(1, adm.Id);
    }

}
