using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;

namespace Vasis.MDFe.Api.RegressionTests
{
    /// <summary>
    /// Testes de regressão para garantir que funcionalidades críticas não sejam quebradas
    /// </summary>
    public class CriticalPathRegressionTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CriticalPathRegressionTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CriticalPath_CompleteAuthenticationFlow_ShouldWork()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            var loginContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(loginRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            // Act - Step 1: Login
            var loginResponse = await _client.PostAsync("/api/Auth/login", loginContent);

            // Assert - Step 1
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginContent_Response = await loginResponse.Content.ReadAsStringAsync();
            loginContent_Response.Should().Contain("token");

            // Extract token
            var loginResult = System.Text.Json.JsonSerializer.Deserialize<dynamic>(loginContent_Response);
            var token = loginResult.GetProperty("token").GetString();

            // Act - Step 2: Use token to access protected endpoint
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var protectedResponse = await _client.GetAsync("/api/MdfeService/source-integration-health");

            // Assert - Step 2
            protectedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var protectedContent = await protectedResponse.Content.ReadAsStringAsync();
            protectedContent.Should().Contain("SUCESSO NA POC DE INTEGRAÇÃO DO FONTE");
        }

        [Fact]
        public async Task CriticalPath_ZeusIntegration_ShouldBeStable()
        {
            // Este teste garante que a integração Zeus não seja quebrada
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            var loginContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(loginRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/api/Auth/login", loginContent);
            var loginContentResponse = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = System.Text.Json.JsonSerializer.Deserialize<dynamic>(loginContentResponse);
            var token = loginResult.GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert - Verificações críticas que não podem falhar
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Zeus DFe.NET");
            content.Should().Contain("MDF-e");
            content.Should().NotContain("erro");
            content.Should().NotContain("exception");
            content.Should().NotContain("falha");
        }

        [Fact]
        public async Task CriticalPath_ApiHealth_ShouldAlwaysBeAvailable()
        {
            // Este teste garante que a API esteja sempre respondendo
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            // Mesmo que retorne 404, significa que a API está rodando
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
    }
}