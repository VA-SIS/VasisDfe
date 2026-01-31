// Top-level statements - Program.cs em ASP.NET Core 8.0
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================================
// INÍCIO: Adição e Configuração de Serviços
// ===================================================================================

// Serviços Padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger/OpenAPI com suporte a JWT
builder.Services.AddSwaggerGen(c =>
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
        Description = "Token de autenticação JWT (Bearer Token). Ex: Bearer {token}"
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
            new string[] {}
        }
    });
});

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração do logger
builder.Services.AddLogging();

// --- INÍCIO DA CONFIGURAÇÃO JWT PARA .NET 8.0 ---
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

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

// Adiciona os serviços de autenticação com JWT Bearer
builder.Services.AddAuthentication(options =>
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

// Adiciona os serviços de autorização
builder.Services.AddAuthorization();
// --- FIM DA CONFIGURAÇÃO JWT ---

// ===================================================================================
// FIM: Adição e Configuração de Serviços
// ===================================================================================

var app = builder.Build();

// ===================================================================================
// INÍCIO: Configuração do Pipeline de Requisições HTTP - ORDEM CORRETA
// ===================================================================================

// Configurações para ambiente de Desenvolvimento E TESTE
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redirecionamento HTTPS
app.UseHttpsRedirection();

// --- ORDEM CORRETA DOS MIDDLEWARES JWT ---
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSwaggerUI");

// Mapeia os controllers da aplicação
app.MapControllers();

app.Run();
// ===================================================================================
// FIM: Configuração do Pipeline de Requisições HTTP
// ===================================================================================

public partial class Program { }