// File: Vasis.MDFe.Api.IntegrationTests\ZeusIntegrationTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System; // Adicionado para Random, se necessário para gerar dados unicos
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers; // Adicionado para MediaTypeHeaderValue
using System.Text;
using System.Threading.Tasks;
using Vasis.MDFe.Api.IntegrationTests; // Adicionado para acessar TestJwtTokenGenerator
using Vasis.MDFe.Api.TestUtilities; // Adicionado para acessar TestDataBuilder
using Xunit;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class ZeusIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ZeusIntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetHealthCheck_ReturnsOk()
        {
            // Ajuste da rota: O endpoint de Health Check no Program.cs da API é "/healthz"
            var response = await _client.GetAsync("/healthz");

            response.EnsureSuccessStatusCode(); // Lança exceção se o código de status não indicar sucesso
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetMDFE_ReturnsUnauthorized_WithoutAuthToken()
        {
            // Ajuste da rota: O endpoint do MDFE na API é "/api/mdfe"
            var response = await _client.GetAsync("/api/mdfe");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WithoutToken()
        {
            // Este teste verifica que um endpoint protegido retorna 401 sem token.
            // O endpoint "/api/protected" pode ser um exemplo genérico para sua API.
            // Se você não tiver um endpoint "/api/protected" genérico, pode usar /api/mdfe.
            // Vou assumir que "/api/protected" não existe, então usaremos "/api/mdfe".
            var response = await _client.GetAsync("/api/mdfe/some-id"); // Exemplo de endpoint protegido

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var loginRequest = TestDataBuilder.BuildLoginRequest();
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/auth/login", content);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNullOrEmpty();

            // O token gerado deve estar na resposta
            // Você pode adicionar mais asserts para validar o formato do token, etc.
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsOk_WithValidToken()
        {
            // 1. Obter um token JWT válido para o usuário de teste.
            // A configuração é injetada via TestWebApplicationFactory.
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            var token = TestJwtTokenGenerator.GenerateToken(config, "testuser-id", "Admin", "testuser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Ajuste da rota: Usar um endpoint protegido real da sua API. Ex: "/api/mdfe/some-id"
            // Assumindo que este endpoint não precisa de um MDF-e existente para apenas testar a autorização.
            // Se /api/mdfe/some-id retornar 404, mude para um endpoint que retorne 200 com autenticação.
            var response = await _client.GetAsync("/api/mdfe"); // Exemplo: Tenta obter todos os MDF-e

            response.EnsureSuccessStatusCode(); // Verifica se o status é 2xx
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        // Exemplo: Teste de Criação de MDF-e
        [Fact]
        public async Task CreateMDFE_ReturnsCreated_WithValidDataAndAuthToken()
        {
            // 1. Obter um token JWT válido.
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            var token = TestJwtTokenGenerator.GenerateToken(config, "testuser-id", "Admin", "testuser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Criar um objeto MDF-e de teste (usando o builder)
            var mdfeCreateRequest = TestDataBuilder.BuildMdfeCreateRequest();
            var content = new StringContent(JsonConvert.SerializeObject(mdfeCreateRequest), Encoding.UTF8, "application/json");

            // 3. Enviar a requisição POST para o endpoint correto.
            // Ajuste da rota: O endpoint para criar MDF-e na API é "/api/mdfe" (POST)
            var response = await _client.PostAsync("/api/mdfe", content);

            // 4. Verificar o StatusCode esperado
            response.EnsureSuccessStatusCode(); // Isso vai falhar se não for 2xx (por exemplo, 404)
            response.StatusCode.Should().Be(HttpStatusCode.Created); // Espera 201 Created

            // Você pode adicionar mais asserts aqui, como validar a resposta, o ID do MDF-e criado, etc.
        }

        // Exemplo: Teste para buscar MDF-e por ID (já estava aprovado)
        [Fact]
        public async Task GetMDFEById_ReturnsNotFound_ForNonExistentId_WithValidAuthToken()
        {
            // 1. Obter um token JWT válido.
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            var token = TestJwtTokenGenerator.GenerateToken(config, "testuser-id", "Admin", "testuser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Tentar buscar um ID que não existe.
            // Ajuste da rota: O endpoint para buscar MDF-e por ID na API é "/api/mdfe/{id}"
            var nonExistentId = Guid.NewGuid(); // Gera um ID aleatório para garantir que não exista
            var response = await _client.GetAsync($"/api/mdfe/{nonExistentId}");

            // 3. Verificar o StatusCode esperado
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetMDFE_ReturnsOk_WithValidAuthToken()
        {
            // 1. Obter um token JWT válido.
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            var token = TestJwtTokenGenerator.GenerateToken(config, "testuser-id", "Admin", "testuser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Chamar o endpoint que lista todos os MDF-e
            // Ajuste da rota: O endpoint para listar MDF-e na API é "/api/mdfe" (GET)
            var response = await _client.GetAsync("/api/mdfe");

            // 3. Verificar o StatusCode esperado
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}