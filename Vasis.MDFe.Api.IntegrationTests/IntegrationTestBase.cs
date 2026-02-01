using Xunit;
using System.Net.Http;
using Microsoft.Extensions.Configuration; // Para IConfiguration
using Microsoft.Extensions.DependencyInjection; // Para GetRequiredService

namespace Vasis.MDFe.Api.IntegrationTests
{
    // Esta classe base garante que cada fixture de teste obtenha sua própria instância de WebApplicationFactory
    // e, portanto, seu próprio HttpClient, promovendo o isolamento dos testes.
    // IClassFixture<T> é uma interface do xUnit.net para criar uma única instância de fixture de teste
    // e compartilhá-la entre todos os testes na classe de teste.
    public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>
    {
        protected readonly TestWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        protected readonly IConfiguration _configuration; // Para acessar as configurações do appsettings.Testing.json

        protected IntegrationTestBase(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            // Obtém o IConfiguration dos serviços da factory.
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        }
    }
}