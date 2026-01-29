using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class AuthenticationIntegrationTests : IntegrationTestBase
    {
        public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
            : base(factory) { }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            // Act
            var response = await _client.PostAsync("/api/Auth/login",
                CreateJsonContent(loginRequest));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
            content.Should().Contain("expiration");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "senhaerrada"
            };

            // Act
            var response = await _client.PostAsync("/api/Auth/login",
                CreateJsonContent(loginRequest));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthorizationHeader(token);

            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("SUCESSO NA POC DE INTEGRAÇÃO DO FONTE");
        }
    }
}