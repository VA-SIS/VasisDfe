using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vasis.MDFe.Api.Services;
using Vasis.MDFe.Api.Services.Interfaces;
using System.Reflection; // Importação adicionada para usar typeof(Program).Assembly
using Microsoft.Extensions.DependencyInjection; // Este 'using' é geralmente suficiente
namespace Vasis.MDFe.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ***** INÍCIO DA CORREÇÃO CRÍTICA PARA DESCOBERTA DE CONTROLADORES *****
            // 1. Obtenha a assembly onde a classe Program (e, portanto, seus controladores) reside.
            // Isso é essencial para que o MVC possa descobrir os controladores quando o AddControllers()
            // é chamado de um projeto de extensão.
            var assemblyWithControllers = typeof(Program).Assembly;

            // 2. Registre os controladores e explicitamente adicione a assembly como um Application Part.
            // Isso força o MVC a procurar controladores dentro da sua assembly principal da API.
            // 3. Adicione .AddNewtonsoftJson() para garantir que a serialização/desserialização JSON use
            // Newtonsoft.Json conforme discutido anteriormente, resolvendo possíveis ambiguidades.
            services.AddControllers()
                    .AddApplicationPart(assemblyWithControllers) // <--- ESSENCIAL PARA OS TESTES 404
                    .AddNewtonsoftJson();                       // <--- GARANTE O USO DE NEWTONSOFT.JSON
            // ***** FIM DA CORREÇÃO CRÍTICA PARA DESCOBERTA DE CONTROLADORES *****

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
            // A chave secreta é buscada da configuração, com um fallback para desenvolvimento.
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
                options.RequireHttpsMetadata = false; // Pode ser 'true' em produção se você gerencia o HTTPS
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "Vasis.MDFe.Api", // Issuer padrão para desenvolvimento
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "aplicacoes_clientes", // Audience padrão para desenvolvimento
                    ValidateLifetime = true, // Valida a expiração do token
                    ClockSkew = TimeSpan.Zero // Remove o 'skew' padrão de 5 minutos na validação do tempo
                };
            });

            return services;
        }
    }
}