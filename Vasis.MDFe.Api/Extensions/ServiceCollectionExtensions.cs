using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vasis.MDFe.Api.Services;
using Vasis.MDFe.Api.Services.Interfaces;
using System.Reflection; // Necessário para typeof(Program).Assembly
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration
using Microsoft.Extensions.DependencyInjection; // Necessário para IServiceCollection

namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Adiciona e configura os controladores.
            // O .AddApplicationPart() garante que os controladores da assembly principal (Vasis.MDFe.Api)
            // sejam descobertos, mesmo que a configuração esteja em uma assembly de extensão.
            services.AddControllers()
                    .AddApplicationPart(typeof(Program).Assembly); // ✅ CORREÇÃO: Força a descoberta de controladores da API.

            services.AddEndpointsApiExplorer();

            // Registro dos serviços da aplicação
            services.AddScoped<IMDFeValidationService, MDFeValidationService>();
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

            return services;
        }
    }
}