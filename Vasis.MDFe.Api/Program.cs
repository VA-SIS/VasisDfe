// Program.cs - VERSÃO MODULARIZADA E LIMPA
using Vasis.MDFe.Api.Extensions; // Importa os novos métodos de extensão

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------
// 1. Configuração dos Serviços (builder.Services)
// -----------------------------------------------------
builder.Services.AddCustomLogging(builder.Configuration);
builder.Services.AddCustomControllers();
builder.Services.AddCustomCors();
builder.Services.AddCustomSwagger(); // Adiciona serviços do Swagger
builder.Services.AddCustomHealthChecks();

// Autenticação JWT e Autorização (inclui a correção do IDX10517)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Serviços da Aplicação (AuthService, MDFeService, TokenService, IHttpClientFactory, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

// -----------------------------------------------------
// 2. Construção da Aplicação
// -----------------------------------------------------
var app = builder.Build();

// -----------------------------------------------------
// 3. Configuração do Pipeline de Requisições (app.Use)
// -----------------------------------------------------
app.UseCustomPipeline(app.Environment); // Inclui Swagger, tratamento de erros, log de requisições, HTTPS, Routing, CORS, Auth, Authz

app.MapControllers();
app.MapHealthChecks("/health");

// Endpoint de teste básico
app.MapGet("/", () => new
{
    Service = "Vasis MDFe API",
    Version = "1.0.0",
    Status = "Running",
    Timestamp = DateTime.UtcNow
});

app.Run();

// Necessário para testes de integração
public partial class Program { }