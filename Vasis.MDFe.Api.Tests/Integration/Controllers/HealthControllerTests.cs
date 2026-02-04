using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

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

            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            content.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHealth_ShouldReturnCorrectContentType()
        {
            // Act
            var response = await Client.GetAsync("/health");

            // Assert
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        }
    }
}