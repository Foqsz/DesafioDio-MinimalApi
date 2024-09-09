using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using DesafioDio_MinimalApi.Project.Domain.Enuns;
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

app.MapGet("/Administradores", async ([FromQuery] int? pagina, IAdminService administradorService) =>
{
    var adms = new List<ListAdminDTO>();

    var adminAll = await administradorService.GetAdminAll(pagina);
    
    foreach (var admin in adminAll)
    {
        adms.Add(new ListAdminDTO
        {
            Email = admin.Email,
            Perfil = admin.Perfil
        });
    }

    return Results.Ok(adms);     

}).WithTags("Administradores");

app.MapGet("/Administradores/{id}", async ([FromRoute] int id, IAdminService administradorService) =>
{
    var adminId = await administradorService.GetAdminById(id);

    if (adminId is null)
    {
        return Results.NotFound("Admin não encontrado");
    }

    return Results.Ok(new ListAdminDTO
    {
        Email = adminId.Email,
        Perfil = adminId.Perfil
    });
}).WithTags("Administradores");

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
}).WithTags("Administradores");

app.MapPost("/Administradores", async ([FromBody] AdminDTO adminDTO, IAdminService administradorService) =>
{
    if (string.IsNullOrEmpty(adminDTO.Email))
    {
        return Results.NotFound("O e-mail não pode ser vazio");
    }

    if (string.IsNullOrEmpty(adminDTO.Senha))
    {
        return Results.NotFound("A senha não pode ficar vazia.");
    }

    if (adminDTO.Perfil == null)
    {
        return Results.NotFound("O Perfil não pode ficar vazio.");
    } 

    var admin = new Admin
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString()
    };

    await administradorService.GetCreateAdmin(admin);

    return Results.Created($"O Administrador {admin} foi registrado com sucesso.", new ListAdminDTO
    {
        Email = admin.Email,
        Perfil = admin.Perfil
    });

}).WithTags("Administradores");
#endregion

#region Veículos

app.MapGet("/veiculos", async ([FromQuery] int? pagina, IVehicleService vehicleService) =>
{
    var vehicles = await vehicleService.GetVehicles(pagina);

    return Results.Ok(vehicles);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", async ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicleId = await vehicleService.GetVehicleId(id);

    if (vehicleId is null)
    {
        return Results.NotFound("Veiculo não encontrado");
    }

    return Results.Ok(vehicleId);
}).WithTags("Veiculos");

app.MapPost("/veiculos", async ([FromBody] VehicleDTO veiculoDTO, IVehicleService vehicleService) =>
{
    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        return Results.NotFound("O Nome não pode ser vazio");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        return Results.NotFound("A marca não pode ficar em branco");
    }

    if (veiculoDTO.Ano < 1950)
    {
        return Results.NotFound("Veículo muito antigo. Aceitando apenas anos superiores a 1950.");
    }

    var vehicle = new Vehicle
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    await vehicleService.GetVehicleCreate(vehicle);

    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", async ([FromRoute] int id, VehicleDTO vehicleDto, IVehicleService vehicleService) =>
{
    var vehicle = await vehicleService.GetVehicleId(id);

    if (vehicle is null)
    {
        return Results.NotFound("Veiculo não encontrado");
    }

    vehicle.Nome = vehicleDto.Nome;
    vehicle.Marca = vehicleDto.Marca;
    vehicle.Ano = vehicleDto.Ano;

    if (string.IsNullOrEmpty(vehicle.Nome))
    {
        return Results.NotFound("O Nome não pode ser vazio");
    }

    if (string.IsNullOrEmpty(vehicle.Marca))
    {
        return Results.NotFound("A marca não pode ficar em branco");
    }

    if (vehicle.Ano < 1950)
    {
        return Results.NotFound("Veículo muito antigo. Aceitando apenas anos superiores a 1950.");
    }

    await vehicleService.GetVehicleUpdate(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", async ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = await vehicleService.GetVehicleId(id);

    if (vehicle is null)
    {
        return Results.NotFound("Veiculo não encontrado");
    }

    await vehicleService.GetVehicleDelete(vehicle);

    return Results.Ok();
}).WithTags("Veiculos");
#endregion

#region App
app.Run();
#endregion
