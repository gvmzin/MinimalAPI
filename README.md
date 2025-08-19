# Minimal API com Arquitetura Limpa em .NET 8

## 📖 Sobre o Projeto

Este projeto é uma demonstração de como desenvolver uma **Minimal API em .NET 8** com foco em uma arquitetura limpa, organizada e escalável. O objetivo é mostrar que é possível utilizar a abordagem de APIs mínimas sem abrir mão da organização do código e da separação de responsabilidades, facilitando a manutenção e a evolução do projeto.

A filosofia é provar que **"Minimal API não precisa ser sinônimo de código desorganizado"**.

-----

## ✨ Features

  - ✅ **Arquitetura Limpa:** Separação clara de responsabilidades em camadas (Domínio, Infraestrutura, Serviços).
  - ✅ **Autenticação e Autorização:** Implementação de segurança com **JWT (JSON Web Tokens)**.
  - ✅ **Injeção de Dependência:** Uso extensivo para desacoplar os componentes da aplicação.
  - ✅ **Entity Framework Core:** Mapeamento objeto-relacional para interação com o banco de dados.
  - ✅ **Documentação de API:** Geração automática de documentação com Swagger/OpenAPI.
  - ✅ **Gerenciamento de CRUDs:** Operações completas para as entidades de Administradores e Veículos.

-----

## 🛠️ Tecnologias Utilizadas

  - **.NET 8**
  - **ASP.NET Core (Minimal API)**
  - **Entity Framework Core 8**
  - **Swagger (Swashbuckle)**
  - **Autenticação JWT**

-----

## 📂 Estrutura do Projeto

O código está organizado em camadas para garantir a separação de conceitos e facilitar a manutenibilidade.

  - **Domain:** Contém as entidades de negócio (`Administrador`, `Veiculo`), Enums e perfis de mapeamento (AutoMapper).
  - **DTOs (Data Transfer Objects):** Objetos para transferência de dados entre as camadas, como `AdministradorDTO`, `LoginDTO` e `VeiculoDTO`.
  - **ModelViews:** Classes que representam os dados de resposta formatados para o cliente.
  - **Interfaces & Services:** Contém as abstrações (interfaces) e suas implementações (services) que orquestram as regras de negócio.
  - **Infrastructure:** Camada responsável pela comunicação com recursos externos, principalmente a configuração do banco de dados com Entity Framework Core (`DbContext`).
  - **Program.cs:** Ponto de entrada da API, onde os serviços são configurados e os endpoints são mapeados de forma organizada utilizando regiões.

-----

## 🚀 Como Executar

Siga os passos abaixo para executar o projeto localmente.
Ou tente o acesso por este [link](http://3.144.152.177/swagger/index.html), caso não esteja funcionando rode localmente.

**Pré-requisitos:**

  * [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  * Um SGBD de sua preferência (ex: SQL Server, PostgreSQL, etc.)

**Passos:**

1.  Clone o repositório:

    ```bash
    git clone https://github.com/gvmzin/MinimalAPI.git
    ```

2.  Navegue até o diretório do projeto:

    ```bash
    cd MinimalAPI
    ```

3.  Configure sua string de conexão com o banco de dados no arquivo `appsettings.json`.

4.  Instale as dependências e execute as migrations (se houver):

    ```bash
    dotnet restore
    dotnet ef database update
    ```

5.  Inicie a aplicação:

    ```bash
    dotnet run
    ```

6.  A API estará disponível em `http://localhost:<porta>`. Você pode acessar a documentação do Swagger em `http://localhost:<porta>/swagger`.

-----

## ⚡ Endpoints da API

Abaixo estão listados os endpoints disponíveis na aplicação.

### Home

  - `GET /`
      - Retorna uma mensagem de boas-vindas da API.

### Administradores

  - `POST /administradores/login`
      - Autentica um administrador e retorna um token JWT.
  - `GET /administradores`
      - Lista todos os administradores cadastrados.
  - `POST /administradores`
      - Cria um novo administrador.
  - `GET /administradores/{id}`
      - Busca um administrador específico pelo seu ID.

### Veículos

  - `POST /veiculos`
      - Cadastra um novo veículo.
  - `GET /veiculos`
      - Lista todos os veículos cadastrados.
  - `GET /veiculos/{id}`
      - Busca um veículo específico pelo seu ID.
  - `PUT /veiculos/{id}`
      - Atualiza os dados de um veículo existente.
  - `DELETE /veiculos/{id}`
      - Remove um veículo do sistema.

-----

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](https://www.google.com/search?q=LICENSE) para mais detalhes.
