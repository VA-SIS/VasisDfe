using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder; // Para IApplicationBuilder
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment
using Microsoft.Extensions.Hosting; // Para IWebHostEnvironment (IsDevelopment)

namespace Vasis.MDFe.Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vasis MDFe API", Version = "v1" });

                // Adiciona o esquema de segurança JWT (Bearer) ao Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    // 🔥 Correção de aspas duplas usando literal de string bruto
                    Description = """Token de autenticação JWT (Bearer Token). Ex: Bearer {token}"""
                });

                // Garante que o Swagger envie o token JWT para os endpoints protegidos
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

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "Testing")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API V1");
                });
            }
            return app;
        }
    }
}