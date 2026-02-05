// C:\\\\\\Zeus\\\\\\1935\\\\\\DFe.NET-2026\\\\\\Vasis.MDFe.Api.Tests\\\\\\Integration\\\\\\Controllers\\\\\\MDFeControllerTests.cs

using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Vasis.MDFe.Api.Tests.Integration;
using Vasis.MDFe.Api.Tests.Fixtures;
using System.Threading.Tasks;

namespace Vasis.MDFe.Api.Tests.Integration.Controllers
{
    public class MDFeControllerTests : TestBase
    {
        public MDFeControllerTests(TestWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetMDFe_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            // Aponta para um endpoint GET da sua API que requer autorização
            var response = await Client.GetAsync("/api/mdfe/status-servico/SP");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMDFe_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = await GetValidTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            // Aponta para um endpoint GET da sua API que requer autorização
            var response = await Client.GetAsync("/api/mdfe/status-servico/SP");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            // Você pode adicionar mais assertions aqui para verificar o conteúdo da resposta se desejar
        }

        [Fact]
        public async Task GetMDFe_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthTestData.InvalidJwtToken);

            // Act
            // Aponta para um endpoint GET da sua API que requer autorização
            var response = await Client.GetAsync("/api/mdfe/status-servico/SP");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private async Task<string> GetValidTokenAsync()
        {
            var loginRequest = AuthTestData.ValidCredentials;

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha ao obter token válido para teste. Status: {response.StatusCode}. Erro: {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            return content.GetProperty("token").GetString() ??
                   throw new InvalidOperationException("Token não encontrado na resposta");
        }
    }
}