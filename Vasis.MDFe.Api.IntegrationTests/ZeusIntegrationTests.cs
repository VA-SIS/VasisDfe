using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using Xunit;
using Vasis.MDFe.Api.TestUtilities;
using System.Net.Http.Headers; // Necessário para AuthenticationHeaderValue
using System.Text; // Necessário para Encoding
using Newtonsoft.Json; // ***** CORREÇÃO: Usando Newtonsoft.Json explicitamente *****
using System.Threading.Tasks; // Necessário para Task
using System; // Necessário para Console.WriteLine, StringComparison, Math
using System.Collections.Generic; // Necessário para Dictionary, se usado

namespace Vasis.MDFe.Api.IntegrationTests
{
    // A fábrica customizada já foi corrigida para ser usada neste e no outro arquivo de teste.
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
        public async Task MDFeEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: MDFeEndpoint_WithValidToken_ShouldReturnSuccess ---");

            // Arrange - Usando token fixo para teste (o token real deve ser gerado pelo Login)
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}api/mdfe");
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = Bearer {token.Substring(0, Math.Min(token.Length, 10))}...");

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("SUCESSO NA POC DE INTEGRAÇÃO DO FONTE", content);
            Console.WriteLine("--- FIM TESTE: MDFeEndpoint_WithValidToken_ShouldReturnSuccess ---\n");
        }

        [Fact]
        public async Task MDFeEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: MDFeEndpoint_WithoutToken_ShouldReturnUnauthorized ---");

            // Arrange
            _client.DefaultRequestHeaders.Authorization = null; // Garante que não há token

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}api/mdfe");
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = null");

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: MDFeEndpoint_WithoutToken_ShouldReturnUnauthorized ---\n");
        }

        [Fact]
        public async Task ZeusLibrary_ShouldBeAccessible()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: ZeusLibrary_ShouldBeAccessible ---");

            // Arrange - Usando token fixo para teste
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}api/mdfe");
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = Bearer {token.Substring(0, Math.Min(token.Length, 10))}...");

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            foreach (var keyword in TestDataBuilder.Assertions.SuccessKeywords)
            {
                Assert.Contains(keyword, content, StringComparison.OrdinalIgnoreCase);
            }
            Console.WriteLine("--- FIM TESTE: ZeusLibrary_ShouldBeAccessible ---\n");
        }

        [Fact]
        public async Task LoginEndpoint_WithValidCredentials_ShouldReturnToken()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: LoginEndpoint_WithValidCredentials_ShouldReturnToken ---");

            // Arrange: Crie um objeto com as credenciais de teste
            var loginRequest = new { username = "testuser", password = "password" }; // Adapte para as credenciais de teste reais da sua API

            // ***** CORREÇÃO: Usando JsonConvert para serializar para o corpo da requisição e para o log *****
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}"); // Logando o corpo serializado

            // Act: Envie a requisição para o endpoint de login
            var response = await _client.PostAsync("/auth/login", content);

            // Log da Resposta
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseBody}");

            // Assert: Verifique se a resposta foi bem-sucedida e contém um token
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("token", responseBody);
            Console.WriteLine("--- FIM TESTE: LoginEndpoint_WithValidCredentials_ShouldReturnToken ---\n");
        }
    }
}