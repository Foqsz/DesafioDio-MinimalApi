using DesafioDio_MinimalApi.Project.Domain.DTOs;
using DesafioDio_MinimalApi.Project.Domain.Entities;
using System.Text;
using System.Text.Json;
using TestMinimalApi.Helpers;

namespace TestMinimalApi.Domain;

[TestClass]
public class AdminRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Setup.ClassInit(context);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarGetSetPropriedades()
    {
        //Arrange 
        var loginDTO = new LoginDTO
        {
            Email = "adm@adm.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

        //Act 
        var response = await Setup.client.PostAsync("/Administradores/login", content);

        //Assert  
        Assert.AreEqual(200, (int)response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();

        var admLogado = JsonSerializer.Deserialize<AdmConnectedDTO>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");
         
    }
}

