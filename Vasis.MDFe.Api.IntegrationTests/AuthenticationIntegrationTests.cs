using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using Newtonsoft.Json;
using Vasis.MDFe.Api.TestUtilities;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "MinhaChaveSuperSecretaParaTestesComMaisDe32Caracteres123456",
                        ["Jwt:Issuer"] = "VasisMDFeApi",
                        ["Jwt:Audience"] = "VasisMDFeApiUsers",
                        ["Jwt:ExpiryInMinutes"] = "60"
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = TestDataBuilder.Auth.ValidLoginRequest;
            var content = TestDataBuilder.Http.CreateJsonContent(loginRequest);

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = TestDataBuilder.Auth.InvalidLoginRequest;
            var content = TestDataBuilder.Http.CreateJsonContent(loginRequest);

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = TestDataBuilder.Auth.EmptyLoginRequest;
            var content = TestDataBuilder.Http.CreateJsonContent(loginRequest);

            // Act
            var response = await _client.PostAsync("/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange - Usando token fixo para teste
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/healthz");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/healthz");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}