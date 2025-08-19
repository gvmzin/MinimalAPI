using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Entidades;
using MinimalAPI.Dominio.Interfaces;
using Test.Mocks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Test.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("http_port", Setup.PORT).UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradoresServicoMock>();




            });
        });
        Setup.client = Setup.http.CreateClient();
    }
    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}
