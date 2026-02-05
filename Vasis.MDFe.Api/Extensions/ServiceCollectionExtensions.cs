// C:\Zeus\1935\DFe.NET-2026\Vasis.MDFe.Api.Extensions\ServiceCollectionExtensions.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Vasis.MDFe.Api.Services; // Para que as classes de HealthCheck sejam encontradas
using Vasis.MDFe.Api.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;

namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                    .AddApplicationPart(typeof(Program).Assembly);

            services.AddEndpointsApiExplorer();

            services.AddHealthChecks()
                    .AddCheck<MDFeValidationServiceHealthCheck>("MDFe Validation Service")
                    .AddCheck<MDFeLifecycleServiceHealthCheck>("MDFe Lifecycle Service");

            // Registro dos serviços da aplicação
            services.AddScoped<IMDFeValidationService, MDFeValidationService>();
            // ✅ CORREÇÃO AQUI: A implementação deve ser a CLASSE concreta, não a interface novamente.
            services.AddScoped<IMDFeLifecycleService, MDFeLifecycleService>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vasis MDFe API",
                    Version = "v1",
                    Description = "API para geração e validação de MDFe baseada no DFe.NET"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando Bearer scheme. Formato: Authorization: Bearer { token }",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Key"] ??
                           "minha-chave-secreta-super-segura-para-desenvolvimento-com-pelo-menos-32-caracteres";

            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "Vasis.MDFe.Api",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "aplicacoes_clientes",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            return services;
        }

        // As classes MDFeValidationServiceHealthCheck e MDFeLifecycleServiceHealthCheck
        // devem estar em um arquivo separado, por exemplo, Vasis.MDFe.Api/Services/HealthChecks.cs
        // ou você pode as colocar aqui, mas a prática recomendada é separá-las.
        // Apenas para garantir que o compilador as encontre, estou as incluindo aqui no escopo para facilitar.
        // Se já as moveu para Services/HealthChecks.cs, você pode remover daqui.
        public class MDFeValidationServiceHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                // Implemente a lógica real de verificação de saúde para o MDFeValidationService aqui.
                bool isServiceOperational = true; // Substitua por sua lógica real
                return Task.FromResult(isServiceOperational ? HealthCheckResult.Healthy("MDFe Validation Service is active.") : HealthCheckResult.Unhealthy("MDFe Validation Service is unhealthy."));
            }
        }

        public class MDFeLifecycleServiceHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                // Implemente a lógica real de verificação de saúde para o MDFeLifecycleService aqui.
                bool isServiceOperational = true; // Substitua por sua lógica real
                return Task.FromResult(isServiceOperational ? HealthCheckResult.Healthy("MDFe Lifecycle Service is active.") : HealthCheckResult.Unhealthy("MDFe Lifecycle Service is unhealthy."));
            }
        }
    }
}