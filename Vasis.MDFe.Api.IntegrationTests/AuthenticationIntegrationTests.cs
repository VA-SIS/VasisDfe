using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using Xunit;
using Newtonsoft.Json; // ***** CORREÇÃO: Usando Newtonsoft.Json explicitamente *****
using Vasis.MDFe.Api.TestUtilities;
using System.Net.Http.Headers; // Necessário para AuthenticationHeaderValue
using System.Text; // Necessário para Encoding
using System.Threading.Tasks; // Necessário para Task
using System; // Necessário para Console.WriteLine, StringComparison, Math
using System.Collections.Generic; // Necessário para Dictionary, se usado

namespace Vasis.MDFe.Api.IntegrationTests
{
    // JÁ CORRIGIDO: USANDO A SUA FÁBRICA DE TESTES CUSTOMIZADA
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
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: Login_WithValidCredentials_ShouldReturnToken ---");

            // Arrange
            var loginRequest = TestDataBuilder.Auth.ValidLoginRequest;

            // ***** CORREÇÃO: Serializando com Newtonsoft.Json.JsonConvert *****
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Log da Resposta
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(responseContent));
            Assert.Contains("token", responseContent, StringComparison.OrdinalIgnoreCase);
            Console.WriteLine("--- FIM TESTE: Login_WithValidCredentials_ShouldReturnToken ---\n");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: Login_WithInvalidCredentials_ShouldReturnUnauthorized ---");

            // Arrange
            var loginRequest = TestDataBuilder.Auth.InvalidLoginRequest;
            // ***** CORREÇÃO: Serializando com Newtonsoft.Json.JsonConvert *****
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Log da Resposta
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: Login_WithInvalidCredentials_ShouldReturnUnauthorized ---\n");
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ShouldReturnBadRequest()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: Login_WithEmptyCredentials_ShouldReturnBadRequest ---");

            // Arrange
            var loginRequest = TestDataBuilder.Auth.EmptyLoginRequest;
            // ***** CORREÇÃO: Serializando com Newtonsoft.Json.JsonConvert *****
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Log da Resposta
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: Login_WithEmptyCredentials_ShouldReturnBadRequest ---\n");
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: ProtectedEndpoint_WithValidToken_ShouldReturnSuccess ---");

            // Arrange - Usando token fixo para teste
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}health"); // Rota corrigida
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = Bearer {token.Substring(0, Math.Min(token.Length, 10))}...");

            // Act
            var response = await _client.GetAsync("/health"); // Rota corrigida

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: ProtectedEndpoint_WithValidToken_ShouldReturnSuccess ---\n");
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized ---");

            // Arrange
            _client.DefaultRequestHeaders.Authorization = null; // Garante que não há token

            // Log da Requisição
            Console.WriteLine($"[Request] URL: {_client.BaseAddress}health"); // Rota corrigida
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = null");

            // Act
            var response = await _client.GetAsync("/health"); // Rota corrigida

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized ---\n");
        }
    }
}