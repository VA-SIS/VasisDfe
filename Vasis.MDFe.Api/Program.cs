// Top-level statements - Program.cs em ASP.NET Core 8.0
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer; // <<<--- GARANTIDO ESTAR AQUI
using Microsoft.OpenApi.Models; // Necessário para OpenApiSecurityScheme e OpenApiSecurityRequirement
using System;
using Microsoft.Extensions.Logging; // Opcional, mas útil se AddLogging for usado explicitamente

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

// *** ADIÇÃO: Configuração CORS para resolver "Failed to fetch" ***
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração do logger para injeção de dependência (usado em controllers ou outros serviços)
builder.Services.AddLogging();

// --- INÍCIO DA CONFIGURAÇÃO JWT PARA .NET 8.0 ---
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Valida se as configurações JWT foram carregadas, CRÍTICO para segurança e estabilidade.
// Se faltar, a aplicação não inicia, evitando débitos técnicos de segurança.
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
});

// Adiciona os serviços de autorização
builder.Services.AddAuthorization();
// --- FIM DA CONFIGURAÇÃO JWT ---

// ===================================================================================
// FIM: Adição e Configuração de Serviços
// ===================================================================================

var app = builder.Build();

// ===================================================================================
// INÍCIO: Configuração do Pipeline de Requisições HTTP - ORDEM CORRIGIDA
// ===================================================================================

// Configurações para ambiente de Desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API V1");
    });
}

// Redirecionamento HTTPS (boa prática de segurança)
app.UseHttpsRedirection();

// --- ORDEM CORRETA DOS MIDDLEWARES JWT ---
// CRÍTICO: UseAuthentication DEVE VIR ANTES de UseAuthorization
// CRÍTICO: CORS deve vir APÓS a autenticação para não interferir
app.UseAuthentication();        // 1º - Identifica usuário via token JWT
app.UseAuthorization();         // 2º - Verifica permissões do usuário identificado
app.UseCors("AllowSwaggerUI");  // 3º - CORS após autenticação (CORREÇÃO APLICADA)

// Mapeia os controllers da aplicação
app.MapControllers();

app.Run();
// ===================================================================================
// FIM: Configuração do Pipeline de Requisições HTTP
// ===================================================================================