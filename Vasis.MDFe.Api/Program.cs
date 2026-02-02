using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vasis.MDFe.Api.Services;
using Vasis.MDFe.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================================
// INÍCIO: Adição e Configuração de Serviços (builder.Services)
// ===================================================================================

// Adiciona serviços básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger/OpenAPI com suporte a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vasis MDFe API",
        Version = "v1",
        Description = "API para geração e validação de MDFe baseada no DFe.NET"
    });

    // Configuração JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: Authorization: Bearer { token }",
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

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração da Autenticação JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ??
               "SUA_CHAVE_SECRETA_MUITO_FORTE_E_LONGA_E_ALEATORIA_QUE_NINGUEM_VAI_ADIVINHAR";

if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
{
    secretKey = "minha-chave-secreta-super-segura-para-desenvolvimento-com-pelo-menos-32-caracteres";
    Console.WriteLine("⚠️ Usando JWT SecretKey padrão para desenvolvimento");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = jwtSettings["Issuer"] ?? "Vasis.MDFe.Api",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "aplicacoes_clientes",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("✅ JWT Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// Registro dos serviços da aplicação
builder.Services.AddScoped<IMDFeValidationService, MDFeValidationService>();
builder.Services.AddScoped<IMDFeLifecycleService, MDFeLifecycleService>();

// Configuração de Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===================================================================================
// FIM: Adição e Configuração de Serviços
// ===================================================================================

var app = builder.Build();

// ===================================================================================
// INÍCIO: Configuração do Pipeline de Requisições HTTP (app.Use)
// ===================================================================================

// Configuração do pipeline para Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

// Pipeline de requisições
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Endpoint de health check
app.MapGet("/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Services = new
    {
        ValidationService = "Active",
        LifecycleService = "Active",
        JwtAuthentication = "Configured"
    }
}).WithTags("Health");

// Endpoint de informações da API
app.MapGet("/", () => new {
    Message = "Vasis MDFe API está funcionando!",
    Version = "1.0.0",
    Documentation = "/swagger",
    Health = "/health",
    Timestamp = DateTime.UtcNow
}).WithTags("Info");

// ===================================================================================
// FIM: Configuração do Pipeline de Requisições HTTP
// ===================================================================================

Console.WriteLine("🚀 Iniciando Vasis MDFe API...");
Console.WriteLine($"📍 Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine("📖 Documentação disponível em: /swagger");
Console.WriteLine("💚 Health check disponível em: /health");

app.Run();

// Classe parcial para testes
public partial class Program { }