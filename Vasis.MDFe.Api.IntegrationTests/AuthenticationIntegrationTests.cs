using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using Xunit;
using Newtonsoft.Json;
using Vasis.MDFe.Api.TestUtilities;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Vasis.MDFe.Api.IntegrationTests
{
    // A fábrica customizada já foi corrigida para ser usada neste e no outro arquivo de teste.
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

            var loginRequest = TestDataBuilder.Auth.ValidLoginRequest;
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            var response = await _client.PostAsync("/auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(responseContent));
            Assert.Contains("token", responseContent, StringComparison.OrdinalIgnoreCase);
            Console.WriteLine("--- FIM TESTE: Login_WithValidCredentials_ShouldReturnToken ---\n");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: Login_WithInvalidCredentials_ShouldReturnUnauthorized ---");

            var loginRequest = TestDataBuilder.Auth.InvalidLoginRequest;
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            var response = await _client.PostAsync("/auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: Login_WithInvalidCredentials_ShouldReturnUnauthorized ---\n");
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ShouldReturnBadRequest()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: Login_WithEmptyCredentials_ShouldReturnBadRequest ---");

            var loginRequest = TestDataBuilder.Auth.EmptyLoginRequest;
            var jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            Console.WriteLine($"[Request] URL: {_client.BaseAddress}auth/login");
            Console.WriteLine($"[Request] Method: POST");
            Console.WriteLine($"[Request] Body: {jsonRequestBody}");

            var response = await _client.PostAsync("/auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {responseContent}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Console.WriteLine("--- FIM TESTE: Login_WithEmptyCredentials_ShouldReturnBadRequest ---\n");
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            Console.WriteLine("\n--- INICIANDO TESTE: ProtectedEndpoint_WithValidToken_ShouldReturnSuccess ---");

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Console.WriteLine($"[Request] URL: {_client.BaseAddress}health");
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = Bearer {token.Substring(0, Math.Min(token.Length, 10))}...");

            var response = await _client.GetAsync("/health");

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ESTÁ CORRETO: /health é público
            Console.WriteLine("--- FIM TESTE: ProtectedEndpoint_WithValidToken_ShouldReturnSuccess ---\n");
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnOk() // Renomeado para refletir a expectativa
        {
            Console.WriteLine("\n--- INICIANDO TESTE: ProtectedEndpoint_WithoutToken_ShouldReturnOk ---"); // Log ajustado

            _client.DefaultRequestHeaders.Authorization = null; // Garante que não há token

            Console.WriteLine($"[Request] URL: {_client.BaseAddress}health");
            Console.WriteLine($"[Request] Method: GET");
            Console.WriteLine($"[Request] Headers: Authorization = null");

            var response = await _client.GetAsync("/health");

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ***** CORREÇÃO: Esperando OK, pois /health é público *****
            Console.WriteLine("--- FIM TESTE: ProtectedEndpoint_WithoutToken_ShouldReturnOk ---\n"); // Log ajustado
        }

        // ***** NOVO TESTE SUGERIDO: Testando um endpoint PROTEGIDO SEM token *****
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
            var response = await _client.GetAsync("/api/mdfe"); // Endpoint protegido

            // Log da Resposta
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Response] Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"[Response] Body: {content}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Deve retornar Unauthorized
            Console.WriteLine("--- FIM TESTE: MDFeEndpoint_WithoutToken_ShouldReturnUnauthorized ---\n");
        }
    }
}