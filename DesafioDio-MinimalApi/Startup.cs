using DesafioDio_MinimalApi;
using DesafioDio_MinimalApi.Project.Domain.Interfaces;
using DesafioDio_MinimalApi.Project.Domain.Services;
using DesafioDio_MinimalApi.Project.Infrastucture.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DesafioDio_MinimalApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString() ?? "";

    }

    private string key;

    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira seu Token Jwt aqui!"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        var connectionString = Configuration.GetConnectionString("MySql");

        services.AddDbContext<MinDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            #region Administradores

            #region Gerar Token Jwt
            string GerarTokenJwt(Admin admin)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", admin.Email),
                    new Claim("Perfil", admin.Perfil),
                    new Claim(ClaimTypes.Role, admin.Perfil),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            #endregion

            endpoints.MapPost("/Administradores/login", async ([FromBody] LoginDTO loginDTO, IAdminService administradorService) =>
            {
                var adm = await administradorService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdmConnectedDTO
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapGet("/Administradores", async ([FromQuery] int? pagina, IAdminService administradorService) =>
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

            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

            endpoints.MapGet("/Administradores/{id}", async ([FromRoute] int id, IAdminService administradorService) =>
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

            endpoints.MapPost("/Administradores", async ([FromBody] AdminDTO adminDTO, IAdminService administradorService) =>
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

            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");
            #endregion

            #region Veículos

            endpoints.MapGet("/veiculos", async ([FromQuery] int? pagina, IVehicleService vehicleService) =>
            {
                var vehicles = await vehicleService.GetVehicles(pagina);
                return Results.Ok(vehicles);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", async ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicleId = await vehicleService.GetVehicleId(id);

                if (vehicleId is null)
                {
                    return Results.NotFound("Veiculo não encontrado");
                }

                return Results.Ok(vehicleId);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

            endpoints.MapPost("/veiculos", async ([FromBody] VehicleDTO veiculoDTO, IVehicleService vehicleService) =>
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", async ([FromRoute] int id, VehicleDTO vehicleDto, IVehicleService vehicleService) =>
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", async ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = await vehicleService.GetVehicleId(id);

                if (vehicle is null)
                {
                    return Results.NotFound("Veiculo não encontrado");
                }

                await vehicleService.GetVehicleDelete(vehicle);

                return Results.Ok();
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");
            #endregion

        });
    }
}
