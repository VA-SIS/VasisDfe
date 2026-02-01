using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// O namespace do seu projeto de testes de integração.
namespace Vasis.MDFe.Api.IntegrationTests
{
    public static class TestJwtTokenGenerator
    {
        /// <summary>
        /// Gera um token JWT para uso em testes de integração, usando as configurações do IConfiguration.
        /// </summary>
        /// <param name="configuration">A instância de IConfiguration (obtida da TestWebApplicationFactory).</param>
        /// <param name="userId">O ID do usuário a ser incluído no token.</param>
        /// <param name="role">A role do usuário a ser incluída no token.</param>
        /// <returns>Um token JWT válido.</returns>
        public static string GenerateToken(IConfiguration configuration, string userId = "test-user-id", string role = "User")
        {
            // Lê as configurações JWT do ambiente de teste (appsettings.Testing.json)
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            // Validação básica para garantir que as configurações foram carregadas
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("Configurações JWT (Key, Issuer, Audience) ausentes ou inválidas em appsettings.Testing.json.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define as claims do token. Adicione outras claims conforme a sua API espera/valida.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), // Subject ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.Name, userId), // Nome do usuário
                new Claim(ClaimTypes.Role, role) // Role do usuário
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}