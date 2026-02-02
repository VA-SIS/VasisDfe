using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vasis.MDFe.Api.Services;
using Vasis.MDFe.Api.Services.Interfaces;

namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adiciona serviços core da aplicação
        /// </summary>
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Registro de serviços MDFe
            services.AddScoped<IMDFeValidationService, MDFeValidationService>();
            services.AddScoped<IMDFeLifecycleService, MDFeLifecycleService>();

            return services;
        }

        /// <summary>
        /// Configura Swagger com suporte a JWT
        /// </summary>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vasis MDFe API",
                    Version = "v1",
                    Description = "API completa para gerenciamento do ciclo de vida do MDFe"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: Authorization: Bearer { token }",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Configura CORS
        /// </summary>
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

        /// <summary>
        /// Configura autenticação JWT
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ??
               configuration["JwtSettings:SecretKey"] ??
               "minha-chave-secreta-super-segura-para-desenvolvimento-com-pelo-menos-32-caracteres";

            // Validação adicional
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
            {
                secretKey = "minha-chave-secreta-super-segura-para-desenvolvimento-com-pelo-menos-32-caracteres";
                Console.WriteLine("⚠️ Usando JWT SecretKey padrão para desenvolvimento");
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            return services;
        }
    }
}