
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;

#region builder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("mysql");

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico AdministradorServico) =>
{
    if (AdministradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administradores");




#endregion

#region Veiculos

 ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO) {
    
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };
    
    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ser vazio");
    }
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("A marca não pode ficar em branco");
    }
    if (veiculoDTO.Ano < 1950)
    {
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");
    }
    return validacao;
}
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico) =>
{
    
    
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    VeiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico VeiculoServico) =>
{

    var veiculos = VeiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromQuery] int id, IVeiculoServico VeiculoServico) =>
{
    var veiculo = VeiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromQuery] int id, VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico) =>
{

    var veiculo = VeiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    VeiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromQuery]int id, IVeiculoServico VeiculoServico) =>
{
    var veiculo = VeiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();


    VeiculoServico.Apagar(veiculo);
    
    return Results.NoContent();
    
}).WithTags("Veiculos");
#endregion

#region app
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion
