using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI.Dominio.Servicos;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoTeste()
    {

        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        if (path == null)
            throw new Exception("Caminho nulo");
        var builder = new ConfigurationBuilder()
        .SetBasePath(path)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("MySql");

        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        return new DbContexto(options);
    }
    [TestMethod]
    public void TestarSalvarOAdm()
    {
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores;");

        var administrador = new Administrador();
        administrador.Id = 1;
        administrador.Email = "teste@teste.com";
        administrador.Senha = "teste";
        administrador.Perfil = "Adm";


        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(administrador);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
        Assert.AreEqual("teste@teste.com", administrador.Email);
        Assert.AreEqual("teste", administrador.Senha);
        Assert.AreEqual("Adm", administrador.Perfil);
    }
    public void TestarBuscaPorId()
    {
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores;");

        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        
        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);  
        var admTeste = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.IsNotNull(admTeste, "BuscaPorId retornou null");
        Assert.AreEqual(1, admTeste.Id);
    }
}