using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Xunit;
using System.IO;

// Usings para geração de JWT
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System;

// Usings para métodos auxiliares HTTP
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vasis.MDFe.Api.IntegrationTests;

/// <summary>
/// Classe base para testes de integração, configurando o ambiente da API para teste.
/// Implementa IClassFixture para o WebApplicationFactory<Program>, que é o host da API para testes.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Cliente HTTP para fazer requisições à API em teste.
    /// </summary>
    protected readonly HttpClient _client; // Mudança: agora é _client (com underscore)

    /// <summary>
    /// Fábrica para criar e configurar a WebApplication da API em teste.
    /// </summary>
    protected readonly WebApplicationFactory<Program> Factory;

    /// <summary>
    /// Instância da configuração para o ambiente de teste, útil para acessar Jwt:Key, etc.
    /// </summary>
    protected readonly IConfiguration Configuration;

    /// <summary>
    /// Construtor para a classe base de testes de integração.
    /// Configura o WebApplicationFactory para o ambiente de teste.
    /// </summary>
    /// <param name="factory">Fábrica de aplicação web injetada pelo xUnit.</param>
    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        // Configura o host da aplicação web para os testes
        Factory = factory.WithWebHostBuilder(builder =>
        {
            // Define o ambiente da aplicação como "Testing".
            builder.UseEnvironment("Testing");

            // Configura como a aplicação carregará suas configurações.
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Limpa as fontes de configuração padrão
                config.Sources.Clear();

                // Obtém o diretório onde o assembly do projeto de testes está localizado.
                var testProjectDir = Path.GetDirectoryName(typeof(IntegrationTestBase).Assembly.Location);

                // Define o caminho base para a configuração como o diretório do projeto de testes.
                config.SetBasePath(testProjectDir);

                // Carrega appsettings.json (opcional)
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                // Carrega appsettings.Test.json
                config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

                // Adiciona variáveis de ambiente
                config.AddEnvironmentVariables();
            });

            // Configura o logging para o ambiente de teste.
            builder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });
        });

        // Cria um cliente HTTP para interagir com a instância da API em teste.
        _client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Obtém a instância da IConfiguration
        Configuration = Factory.Services.GetRequiredService<IConfiguration>();
    }

    /// <summary>
    /// Gera um token JWT de teste usando as configurações do ambiente de teste.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser incluído na claim 'sub'.</param>
    /// <param name="userName">O nome do usuário a ser incluído na claim 'name'.</param>
    /// <returns>Uma string representando o token JWT gerado.</returns>
    protected string GenerateTestToken(string userId = "testuser", string userName = "Test User")
    {
        // Recupera as configurações JWT carregadas do appsettings.Test.json
        var jwtKey = Configuration["Jwt:Key"];
        var jwtIssuer = Configuration["Jwt:Issuer"];
        var jwtAudience = Configuration["Jwt:Audience"];

        // Validação crucial: Garante que as chaves JWT estão configuradas.
        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("Configurações JWT (Key, Issuer, Audience) estão incompletas ou ausentes em appsettings.Test.json.");
        }

        // Cria a chave de segurança a partir da chave JWT configurada.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Define as claims (declarações) a serem incluídas no token JWT.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, userName),
        };

        // Descreve o token a ser criado.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials
        };

        // Cria e serializa o token JWT.
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Obtém um token de autenticação válido para usar nos testes.
    /// </summary>
    /// <returns>Token JWT válido como string.</returns>
    protected async Task<string> GetAuthTokenAsync()
    {
        // Para testes, simplesmente gera um token válido
        // Em um cenário real, isso poderia fazer uma chamada para /api/auth/login
        return await Task.FromResult(GenerateTestToken());
    }

    /// <summary>
    /// Define o cabeçalho de autorização no cliente HTTP.
    /// </summary>
    /// <param name="token">Token JWT para autorização.</param>
    protected void SetAuthorizationHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Cria conteúdo JSON para requisições HTTP.
    /// </summary>
    /// <param name="obj">Objeto a ser serializado para JSON.</param>
    /// <returns>StringContent com o JSON e Content-Type correto.</returns>
    protected StringContent CreateJsonContent(object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Remove o cabeçalho de autorização do cliente HTTP.
    /// </summary>
    protected void ClearAuthorizationHeader()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }
}