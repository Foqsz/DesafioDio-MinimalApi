using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Domain.Services;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var connectionString = builder.Configuration.GetConnectionString("mysql");

builder.Services.AddDbContext<MinDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();
#endregion

#region Swagger
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
#endregion

#region Administradores
app.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdminService administradorService) =>
{
    if (administradorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login Efetuado!");
    }
    else
    {
        return Results.Unauthorized();
    }
});
#endregion

#region Veículos
app.MapPost("/veiculos", async ([FromBody] VehicleDTO veiculoDTO, IVehicleService vehicleService) =>
{

    var vehicle = new Vehicle
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    await vehicleService.GetVehicleCreate(vehicle);

    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);
});

app.MapGet("/veiculos", async ([FromQuery]int? pagina, IVehicleService vehicleService) =>
{
    var vehicles = await vehicleService.GetVehicles(pagina);

    return Results.Ok(vehicles);
});
#endregion

#region App
app.Run();
#endregion
