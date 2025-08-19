using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
       
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
        
    }
    private string key;
    public IConfiguration Configuration { get; set; } = default!;

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
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no campo abaixo: ",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {   {
                    new OpenApiSecurityScheme{
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

        var connectionString = Configuration.GetConnectionString("MySql");

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

            #endregion

            #region Administradores

            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil)
                };
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }



            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico AdministradorServico) =>
            {
                var adm  = AdministradorServico.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdministradorLogado
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

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico AdministradorServico) =>
            {
                var adms = new List<AdministradorModelView>();
                var administradores = AdministradorServico.Todos(pagina);
                foreach(var adm in administradores)
                {
                    adms.Add(new AdministradorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil =  adm.Perfil
                    });
                }
                return Results.Ok(adms);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");

            endpoints.MapGet("/administradores/{id}", ([FromQuery] int id, IAdministradorServico AdministradorServico) =>
            {
                var administrador = AdministradorServico.BuscaPorId(id);

                if (administrador == null) return Results.NotFound();

                return Results.Ok(new AdministradorModelView
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil =  administrador.Perfil
                    });

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");

            endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico AdministradorServico) =>
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };
                if (string.IsNullOrEmpty(administradorDTO.Email))
                {
                    validacao.Mensagens.Add("O email não pode ser vazio");
                }
                if (string.IsNullOrEmpty(administradorDTO.Senha))
                {
                    validacao.Mensagens.Add("A Senha não pode ser vazia");
                }
                if (administradorDTO.Perfil== null)
                {
                    validacao.Mensagens.Add("O perfil não pode ser vazio");
                }
                if (validacao.Mensagens.Count > 0)
                    {
                        return Results.BadRequest(validacao);
                    }

                    
                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                AdministradorServico.Incluir(administrador);
                
                return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil =  administrador.Perfil
                    });
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");




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
            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico) =>
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
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor"})
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico VeiculoServico) =>
            {

                var veiculos = VeiculoServico.Todos(pagina);
                return Results.Ok(veiculos);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromQuery] int id, IVeiculoServico VeiculoServico) =>
            {
                var veiculo = VeiculoServico.BuscaPorId(id);

                if (veiculo == null) return Results.NotFound();

                return Results.Ok(veiculo);

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor"})
            .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromQuery] int id, VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico) =>
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

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
            .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromQuery]int id, IVeiculoServico VeiculoServico) =>
            {
                var veiculo = VeiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();


                VeiculoServico.Apagar(veiculo);
                
                return Results.NoContent();
                
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
            .WithTags("Veiculos");
            #endregion
        });
    }
}
