using System.Text.Json;

namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(configuration.GetValue<LogLevel>("Logging:LogLevel:Default", LogLevel.Information));
                // Configurações de logging específicas para ASP.NET Core e JWT
                builder.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
                builder.AddFilter("Microsoft.AspNetCore.Authentication.JwtBearer", LogLevel.Information);
                builder.AddFilter("Microsoft.IdentityModel.Tokens", LogLevel.Information);
            });
            return services;
        }

        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurações de Options Pattern
            services.Configure<ZeusConfig>(configuration.GetSection("Zeus"));
            // JwtSettings já é configurada em AddJwtAuthentication

            // Registro de Serviços da Aplicação
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMDFeService, MDFeService>();
            services.AddScoped<ITokenService, TokenService>();

            // Configuração de HttpClient para serviços externos
            services.AddHttpClient();

            return services;
        }
    }
}