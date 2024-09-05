using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Domain.Services;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdminService, AdminService>();

var connectionString = builder.Configuration.GetConnectionString("mysql");

builder.Services.AddDbContext<MinDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService administradorService) =>
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

app.Run();

