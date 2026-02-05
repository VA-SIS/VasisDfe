using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;
using Vasis.MDFe.Api.Tests.Integration; // ✅ Adicionado

namespace Vasis.MDFe.Api.Tests.Integration.Controllers
{
    public class HealthControllerTests : TestBase
    {
        public HealthControllerTests(TestWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetHealth_ShouldReturnOk_WithCorrectStructure()
        {
            // Act
            var response = await Client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            jsonContent.Should().NotBeNullOrEmpty();

            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            content.GetProperty("Status").GetString().Should().Be("Healthy");
            content.GetProperty("Version").GetString().Should().Be("1.0.0");
            content.TryGetProperty("Timestamp", out _).Should().BeTrue();
            content.TryGetProperty("Services", out _).Should().BeTrue();
        }

        [Fact]
        public async Task GetHealth_ShouldReturnCorrectContentType()
        {
            // Act
            var response = await Client.GetAsync("/health");

            // Assert
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        }

        [Fact]
        public async Task GetHealth_ShouldReturnValidTimestamp()
        {
            // Act
            var response = await Client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var timestampString = content.GetProperty("Timestamp").GetString();
            timestampString.Should().NotBeNullOrEmpty();

            // Verificar se é um timestamp válido
            DateTime.TryParse(timestampString, out var timestamp).Should().BeTrue();
            timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetHealth_ShouldReturnServicesStatus()
        {
            // Act
            var response = await Client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            var services = content.GetProperty("Services");
            services.GetProperty("ValidationService").GetString().Should().Be("Active");
            services.GetProperty("LifecycleService").GetString().Should().Be("Active");
        }
    }
}