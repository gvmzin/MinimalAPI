using MinimalApi.DTOs;
using MinimalApi.Dominio.Entidades;


namespace MinimalAPI.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    List<Administrador> Todos(int id);
    Administrador? BuscaPorId(int id);
}