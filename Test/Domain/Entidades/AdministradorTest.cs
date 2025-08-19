using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var administrador = new Administrador();


        // Act
        administrador.Id = 1;
        administrador.Email = "teste@teste.com";
        administrador.Senha = "teste";
        administrador.Perfil = "Perfil";

        // Assert
        Assert.AreEqual(1, administrador.Id);
        Assert.AreEqual("teste@teste.com", administrador.Email);
        Assert.AreEqual("teste", administrador.Senha);
        Assert.AreEqual("Perfil", administrador.Perfil);
    }
}