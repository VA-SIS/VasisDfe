using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using Xunit;
using Vasis.MDFe.Api.TestUtilities;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class ZeusIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ZeusIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task MDFeEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange - Usando token fixo para teste
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("SUCESSO NA POC DE INTEGRAÇÃO DO FONTE", content);
        }

        [Fact]
        public async Task MDFeEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ZeusLibrary_ShouldBeAccessible()
        {
            // Arrange - Usando token fixo para teste
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJUZXN0IFVzZXIiLCJpYXQiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/mdfe");

            // Assert
            var content = await response.Content.ReadAsStringAsync();

            foreach (var keyword in TestDataBuilder.Assertions.SuccessKeywords)
            {
                Assert.Contains(keyword, content, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}