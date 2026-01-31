using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.IdentityModel.Tokens.Jwt; // Necessário para SecurityTokenExpiredException

namespace Vasis.MDFe.Api.Extensions
{
    public static class JwtConfigurationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("Configuração 'Jwt:Key' ausente. Verifique seu secrets.json ou variáveis de ambiente.");
            }
            if (string.IsNullOrEmpty(jwtIssuer))
            {
                throw new InvalidOperationException("Configuração 'Jwt:Issuer' ausente. Verifique seu secrets.json ou variáveis de ambiente.");
            }
            if (string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("Configuração 'Jwt:Audience' ausente. Verifique seu secrets.json ou variáveis de ambiente.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                // Eventos de Logging Detalhado do JWT
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError(context.Exception, "--- JWT Authentication Failed ---. Razão: {message}", context.Exception?.Message);
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            logger.LogError("Token expirado: {Expires}", context.Exception.Message);
                        }
                        else if (context.Exception is SecurityTokenValidationException)
                        {
                            logger.LogError("Falha na validação do token: {Details}", context.Exception.Message);
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogInformation("--- JWT Token Validated Successfully --- para o usuário: {username}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogWarning("--- JWT Challenge Triggered ---. Motivo: {error}, Descrição: {description}. Detalhes da Falha: {details}",
                                          context.Error, context.ErrorDescription, context.AuthenticateFailure?.Message);
                        if (context.AuthenticateFailure != null)
                        {
                            logger.LogError(context.AuthenticateFailure, "Exceção no desafio de autenticação JWT.");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(); // Adiciona os serviços de autorização
            return services;
        }
    }
}