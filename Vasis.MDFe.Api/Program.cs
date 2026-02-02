using Vasis.MDFe.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================================
// INÍCIO: Adição e Configuração de Serviços (builder.Services)
// ===================================================================================

// Adiciona serviços "core" da aplicação (Controllers, EndpointsApiExplorer, Logging)
builder.Services.AddCoreServices(builder.Configuration);

// Configuração do Swagger/OpenAPI com suporte a JWT
builder.Services.AddCustomSwagger();

// Configuração CORS
builder.Services.AddCustomCors();

// Configuração e Adição da Autenticação JWT (incluindo validação de config e eventos)
builder.Services.AddJwtAuthentication(builder.Configuration);

// ===================================================================================
// FIM: Adição e Configuração de Serviços
// ===================================================================================

var app = builder.Build();

// ===================================================================================
// INÍCIO: Configuração do Pipeline de Requisições HTTP (app.Use)
// ===================================================================================

// Configura o pipeline da aplicação
app.ConfigureRequestPipeline();

// ===================================================================================
// FIM: Configuração do Pipeline de Requisições HTTP
// ===================================================================================

app.Run();

public partial class Program { }