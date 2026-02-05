// C:\\\\\\Zeus\\\\\\1935\\\\\\DFe.NET-2026\\\\\\Vasis.MDFe.Api.Tests\\\\\\Integration\\\\\\Controllers\\\\\\AuthControllerTests.cs

using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Vasis.MDFe.Api.Tests.Fixtures;
using Vasis.MDFe.Api.Tests.Integration;

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
            var loginRequest = AuthTestData.ValidCredentials;
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            content.TryGetProperty("token", out var tokenProperty).Should().BeTrue();
            tokenProperty.GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginRequest = AuthTestData.InvalidCredentials;
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ShouldReturnUnauthorized()
        {
            var loginRequest = AuthTestData.EmptyCredentials;
            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithNullBody_ShouldReturnUnauthorized()
        {
            var requestContent = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/auth/login", requestContent);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}