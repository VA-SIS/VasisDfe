using Microsoft.AspNetCore.Builder; // Necessário para WebApplication
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Adiciona Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(configuration.GetValue<LogLevel>("Logging:LogLevel:Default", LogLevel.Information));
            });

            // Adiciona Controllers com opções JSON
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            services.AddEndpointsApiExplorer(); // Para o Swagger funcionar

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSwaggerUI", policy => // Nome da política conforme seu Program.cs original
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            return services;
        }

        // Se houver outros serviços customizados (ex: suas interfaces/implementações de MDFe, Auth, Token),
        // eles seriam adicionados aqui ou em outro método de extensão específico para serviços de domínio.
        // Por enquanto, vamos manter apenas o que está no seu Program.cs original para compilar.
    }
}