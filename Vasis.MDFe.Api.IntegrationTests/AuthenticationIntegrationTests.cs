using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net; // Para HttpStatusCode
using System; // Para StringComparison

namespace Vasis.MDFe.Api.IntegrationTests
{
    // Herda de IntegrationTestBase para obter acesso à factory, ao client e à configuração.
    public class AuthenticationIntegrationTests : IntegrationTestBase
    {
        // Construtor: É crucial para o xUnit injetar o TestWebApplicationFactory.
        public AuthenticationIntegrationTests(TestWebApplicationFactory factory) : base(factory)
        {
            // Qualquer setup específico para AuthenticationIntegrationTests, se necessário.
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // IMPORTANTE: Substitua pelos dados de login que sua API REALMENTE espera.
            // Se sua API tem um endpoint de login, este teste deve usá-lo.
            var loginRequest = new { Username = "testuser", Password = "password" };

            // Serializa o objeto para JSON, usando camelCase (comum em APIs RESTful).
            var jsonContent = JsonSerializer.Serialize(loginRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // IMPORTANTE: Substitua "/api/auth/login" pelo endpoint real de login da sua API.
            var response = await _client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode(); // Espera um código de status 2xx (Ok, Created, etc.)
            var responseString = await response.Content.ReadAsStringAsync();

            // Assumindo que sua resposta de login retorna um objeto JSON com uma propriedade "token".
            Assert.Contains("token", responseString, StringComparison.OrdinalIgnoreCase);

            // Opcional: Deserializar a resposta para verificar a estrutura do token.
            // Exemplo:
            // var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            // Assert.NotNull(loginResponse?.Token);
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WithoutToken()
        {
            // Garante que nenhum cabeçalho de autorização seja enviado para este teste.
            _client.DefaultRequestHeaders.Authorization = null;

            // IMPORTANTE: Substitua "/api/protected" por um endpoint protegido real em sua API.
            var response = await _client.GetAsync("/api/protected");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Espera 401 Unauthorized
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsOk_WithValidToken()
        {
            // Gera um token JWT válido usando nosso TestJwtTokenGenerator e as configurações do appsettings.Testing.json.
            // Ajuste o userId e a role conforme a lógica de autorização da sua API.
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "testuser", role: "User");

            // Anexa o token ao cabeçalho de Autorização do nosso HttpClient.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // IMPORTANTE: Substitua "/api/protected" por um endpoint protegido real em sua API.
            var response = await _client.GetAsync("/api/protected");

            response.EnsureSuccessStatusCode(); // Espera um código de status 2xx
        }
    }
}