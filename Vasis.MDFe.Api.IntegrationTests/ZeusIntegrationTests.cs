using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class ZeusIntegrationTests : IntegrationTestBase
    {
        public ZeusIntegrationTests(WebApplicationFactory<Program> factory)
            : base(factory) { }

        [Fact]
        public async Task SourceIntegrationHealth_ShouldValidateZeusConfiguration()
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
            content.Should().Contain("Zeus DFe.NET");
            content.Should().Contain("MDF-e");
        }

        [Fact]
        public async Task ZeusConfiguration_ShouldLoadCertificateCorrectly()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthorizationHeader(token);

            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("certificado foi configurado");
        }

        [Fact]
        public async Task ZeusConfiguration_ShouldValidateSchemaPath()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthorizationHeader(token);

            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotContain("erro");
            content.Should().NotContain("falha");
            content.Should().NotContain("exception");
        }

        [Fact]
        public async Task ApiResponse_ShouldHaveCorrectContentType()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthorizationHeader(token);

            // Act
            var response = await _client.GetAsync("/api/MdfeService/source-integration-health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
        }
    }
}