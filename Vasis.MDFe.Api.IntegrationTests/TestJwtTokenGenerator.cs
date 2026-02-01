using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public static class TestJwtTokenGenerator
    {
        /// <summary>
        /// Gera um token JWT para uso em testes de integração.
        /// As configurações (Key, Issuer, Audience) são lidas de 'JwtSettings' no IConfiguration.
        /// </summary>
        /// <param name="configuration">A instância de IConfiguration (normalmente obtida do TestWebApplicationFactory).</param>
        /// <param name="userId">O ID do usuário a ser incluído no token (ClaimTypes.NameIdentifier).</param>
        /// <param name="role">A função do usuário a ser incluída no token (ClaimTypes.Role).</param>
        /// <param name="username">O nome de usuário a ser incluído no token (ClaimTypes.Name). Se nulo, usa o userId.</param>
        /// <returns>Uma string representando o token JWT gerado.</returns>
        /// <exception cref="InvalidOperationException">Lançada se as configurações JWT não estiverem corretamente definidas.</exception>
        public static string GenerateToken(IConfiguration configuration, string userId, string role, string username = null)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var keyString = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JwtSettings não estão configurados corretamente em appsettings.Testing.json. Verifique 'Key', 'Issuer' e 'Audience'.");
            }

            var key = Encoding.ASCII.GetBytes(keyString);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username ?? userId), // Usa userId se username não for fornecido
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60), // Token válido por 60 minutos
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}