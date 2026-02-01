using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// O namespace do seu projeto de testes de integração.
namespace Vasis.MDFe.Api.IntegrationTests
{
    // Agora herda da nossa classe base, que já implementa IClassFixture<TestWebApplicationFactory>
    public class AuthenticationIntegrationTests : IntegrationTestBase
    {
        // O _client já é fornecido pela IntegrationTestBase, não precisamos declará-lo novamente
        // O construtor da IntegrationTestBase cuida da injeção do factory e do client

        // O construtor desta classe (se necessário para algo específico dela)
        // precisa chamar o construtor da base:
        public AuthenticationIntegrationTests(TestWebApplicationFactory factory) : base(factory)
        {
            // Qualquer setup específico para AuthenticationIntegrationTests, se houver
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var loginRequest = new { Username = "testuser", Password = "password" };
            var jsonContent = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseString); // Verifique se sua API realmente retorna "token" na resposta de login
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsUnauthorized_WithoutToken()
        {
            // Garante que não há cabeçalho de autorização para este teste
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/protected");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_ReturnsOk_WithValidToken()
        {
            // Geramos o token usando o TestJwtTokenGenerator e as configs do appsettings.Testing.json
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, "testuser", "User");

            // Anexamos o token ao cabeçalho de autorização do nosso _client
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/protected");

            response.EnsureSuccessStatusCode(); // Verifica se o status é 2xx (Ok, Created, etc.)
        }
    }
}