using System.Net.Http;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration

// O namespace do seu projeto de testes de integração.
namespace Vasis.MDFe.Api.IntegrationTests
{
    // IClassFixture<TFixture> é uma interface do xUnit que permite compartilhar o estado
    // de uma classe fixture (nossa TestWebApplicationFactory) entre todos os testes de uma classe.
    public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>
    {
        protected readonly HttpClient _client;
        protected readonly TestWebApplicationFactory _factory;
        protected readonly IConfiguration _configuration; // Para acessar as configurações do appsettings.Testing.json

        public IntegrationTestBase(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Cria um HttpClient conectado à instância da API em memória

            // Obtém o IConfiguration do serviço da API em teste.
            // Isso nos permitirá ler as configurações JWT que a API está usando nos testes.
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        }
    }
}