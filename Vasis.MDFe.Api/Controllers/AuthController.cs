using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // ← ADICIONE ESTE USING
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;

namespace Vasis.MDFe.Api.Controllers
{
    // DTOs (mantém como estão - perfeitos)
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous] // ← ÚNICA MUDANÇA NECESSÁRIA
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            // ... TODO O SEU CÓDIGO EXISTENTE (está perfeito, não mude nada)
            
            if (request.Username != "admin" || request.Password != "senhaforte123")
            {
                _logger.LogWarning($"Tentativa de login falha para o usuário: {request.Username}");
                return Unauthorized("Credenciais inválidas.");
            }

            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                _logger.LogError("Configurações JWT ausentes ou inválidas durante a geração do token. Verifique secrets.json.");
                return StatusCode(500, "Configuração de segurança JWT inválida no servidor.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("CustomClaim", "ValorPersonalizado")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation($"Login bem-sucedido para o usuário: {request.Username}. Token gerado.");

            return Ok(new AuthResponseDto { Token = tokenString, Expiration = expiration });
        }
    }
}