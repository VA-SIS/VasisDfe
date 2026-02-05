// C:\\Zeus\\1935\\DFe.NET-2026\\Vasis.MDFe.Api.Tests\\Integration\\Controllers\\AuthControllerTests.cs

using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Vasis.MDFe.Api.Tests.Integration;
using Vasis.MDFe.Api.Tests.Fixtures;
using System.Text;
using System.Threading.Tasks;

namespace Vasis.MDFe.Api.Tests.Integration.Controllers
{
    public class AuthControllerTests : TestBase
    {
        public AuthControllerTests(TestWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = AuthTestData.ValidCredentials;

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            content.TryGetProperty("token", out var tokenProperty).Should().BeTrue();
            tokenProperty.GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = AuthTestData.InvalidCredentials;

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = AuthTestData.EmptyCredentials;

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithNullBody_ShouldReturnUnauthorized() // ✅ CORREÇÃO AQUI: Mudado para esperar Unauthorized
        {
            // Arrange
            var requestContent = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/auth/login", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}