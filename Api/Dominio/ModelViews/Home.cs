using Microsoft.AspNetCore.SignalR;

namespace MinimalAPI.Dominio.ModelViews;

public struct Home
{
    public string Mensagem { get => "Bem vindo a API de veiculos - MinimalAPI"; }
    public string Doc { get => "/swagger"; }
}