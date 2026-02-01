// File: Vasis.MDFe.Api.IntegrationTests\AuthenticationIntegrationTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection; // Para GetRequiredService
using Microsoft.Extensions.Configuration; // Para IConfiguration
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Vasis.MDFe.Api.IntegrationTests; // Para TestJwtTokenGenerator
using Vasis.MDFe.Api.TestUtilities; // Para TestDataBuilder

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class AuthenticationIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthenticationIntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var loginRequest = TestDataBuilder.BuildLoginRequest();
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Endpoint de login é "/auth/login"
            var response = await _client.PostAsync("/auth/login", content);

            response.EnsureSuccessStatusCode(); // Isso lança exceção se o status code não for 2xx
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNullOrEmpty();
            // Adicione mais asserts se quiser validar o conteúdo do token ou a estrutura da resposta
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WithoutToken()
        {
            // Endpoint protegido de exemplo (pode ser qualquer rota que exija autenticação, por exemplo, um endpoint da API Mdfe)
            // Use uma rota que exista, mas que precise de autenticação.
            var response = await _client.GetAsync("/api/mdfe"); // Exemplo: tenta acessar o GET /api/mdfe sem token

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsOk_WithValidToken()
        {
            // 1. Obter um token JWT válido para o usuário de teste.
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            var token = TestJwtTokenGenerator.GenerateToken(config, "testuser-id", "Admin", "testuser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Chamar um endpoint protegido que DEVE retornar OK com um token válido.
            // Exemplo: um GET para listar MDF-e ou um endpoint simples para verificar autenticação.
            var response = await _client.GetAsync("/api/mdfe"); // Exemplo: Endpoint GET /api/mdfe

            response.EnsureSuccessStatusCode(); // Verifica se o status é 2xx
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}