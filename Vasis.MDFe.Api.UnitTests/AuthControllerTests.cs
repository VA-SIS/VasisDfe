using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Vasis.MDFe.Api.Controllers;
using Xunit;

namespace Vasis.MDFe.Api.UnitTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            // Setup configuração JWT mock
            _mockConfiguration.Setup(x => x["Jwt:Key"])
                .Returns("SUA_CHAVE_SECRETA_MUITO_FORTE_E_LONGA_E_ALEATORIA_QUE_NINGUEM_VAI_ADIVINHAR");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"])
                .Returns("Vasis.MDFe.Api");
            _mockConfiguration.Setup(x => x["Jwt:Audience"])
                .Returns("aplicacoes_clientes");

            _controller = new AuthController(_mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeOfType<AuthResponseDto>();

            var authResponse = okResult?.Value as AuthResponseDto;
            authResponse?.Token.Should().NotBeNullOrEmpty();
            authResponse?.Expiration.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "senhaerrada"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_WithNullRequest_ShouldHandleGracefully()
        {
            // Arrange
            LoginRequestDto loginRequest = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _controller.Login(loginRequest));
        }

        [Theory]
        [InlineData("", "senhaforte123")]
        [InlineData("admin", "")]
        [InlineData("", "")]
        public void Login_WithEmptyCredentials_ShouldReturnUnauthorized(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}