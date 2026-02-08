using Vasis.MDFe.Api.Extensions;
using Vasis.MDFe.Configuration; // Adicionar este using
using System.IO; // Adicionar este using
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder; // Adicionar este using para WebApplication
using Microsoft.Extensions.Configuration; // Adicionar este using para IConfiguration

var builder = WebApplication.CreateBuilder(args);

// ===================================================================================
// INÍCIO: Adição e Configuração de Serviços (builder.Services)
// ===================================================================================

// Adiciona serviços "core" da aplicação (Controllers, EndpointsApiExplorer, Logging)
builder.Services.AddCoreServices(builder.Configuration);

// ===================================================================================
// INÍCIO: Configuração do MDF-e (Bloco Integrado)
// ===================================================================================

// Obter a seção de configuração "VasisMDFe" do appsettings.json
var vasisMDFeConfigurationSection = builder.Configuration.GetSection("VasisMDFe");

// Vincular a seção à classe ConfiguracaoMDFe
// Usamos .Bind() para preencher as propriedades que vêm diretamente do JSON.
// Depois, usaremos a instância concreta 'vasisMDFeConfig' para resolver os caminhos e registrá-la.
var vasisMDFeConfig = new ConfiguracaoMDFe();
vasisMDFeConfigurationSection.Bind(vasisMDFeConfig);

// Obter o diretório base da aplicação.
// Isso será o diretório onde o executável/DLL principal da sua API está, garantindo portabilidade.
var appBaseDirectory = AppContext.BaseDirectory;

// --- RESOLUÇÃO E CRIAÇÃO DE CAMINHOS ABSOLUTOS DINAMICAMENTE ---
if (vasisMDFeConfig != null)
{
    // 1. Certificado Digital
    if (!vasisMDFeConfig.CertificadoDigital.UsaWindowsStore &&
        !string.IsNullOrWhiteSpace(vasisMDFeConfig.CertificadoDigital.NomeArquivoCertificado))
    {
        // Define que a pasta 'cert' estará na raiz do diretório base da aplicação
        var certDirectory = Path.Combine(appBaseDirectory, "cert");
        // Garante que o diretório 'cert' exista
        if (!Directory.Exists(certDirectory))
        {
            Directory.CreateDirectory(certDirectory);
        }
        vasisMDFeConfig.CertificadoDigital.CaminhoCompletoArquivo =
            Path.Combine(certDirectory, vasisMDFeConfig.CertificadoDigital.NomeArquivoCertificado);
    }

    // 2. Diretório de Schemas
    if (!string.IsNullOrWhiteSpace(vasisMDFeConfig.SistemaDFe.PastaSchemas))
    {
        var schemasDirectory = Path.Combine(appBaseDirectory, vasisMDFeConfig.SistemaDFe.PastaSchemas);
        // Garante que o diretório de schemas exista
        if (!Directory.Exists(schemasDirectory))
        {
            Directory.CreateDirectory(schemasDirectory);
        }
        vasisMDFeConfig.SistemaDFe.CaminhoCompletoSchemas = schemasDirectory;
    }

    // 3. Diretório para Salvar XMLs (apenas se a funcionalidade estiver ativa)
    if (vasisMDFeConfig.SistemaDFe.IsSalvarXml &&
        !string.IsNullOrWhiteSpace(vasisMDFeConfig.SistemaDFe.PastaSalvarXml))
    {
        var xmlSaveDirectory = Path.Combine(appBaseDirectory, vasisMDFeConfig.SistemaDFe.PastaSalvarXml);
        // Garante que o diretório para salvar XMLs exista
        if (!Directory.Exists(xmlSaveDirectory))
        {
            Directory.CreateDirectory(xmlSaveDirectory);
        }
        vasisMDFeConfig.SistemaDFe.CaminhoCompletoSalvarXml = xmlSaveDirectory;
    }

    // Registrar a instância JÁ RESOLVIDA E PREENCHIDA de ConfiguracaoMDFe no contêiner de DI.
    // Isso garante que outras partes da aplicação (serviços, controllers) recebam o objeto
    // ConfiguracaoMDFe com todos os CaminhoCompleto... preenchidos corretamente.
    builder.Services.AddSingleton(vasisMDFeConfig);

    // Opcional, mas RECOMENDADO: Validar as configurações logo na inicialização.
    // Isso evita que a aplicação inicie com configurações inválidas.
    if (!vasisMDFeConfig.IsValid())
    {
        // Usar um provedor de serviço temporário para obter o logger
        var serviceProvider = builder.Services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError("FATAL: As configurações do VasisMDFe são inválidas. Verifique o 'appsettings.json' e a integridade dos dados da empresa e caminhos de arquivos/pastas (cert, schemas, xmls).");
        // Se as configurações forem críticas e a aplicação não puder operar sem elas:
        throw new InvalidOperationException("Falha na inicialização: Configurações do VasisMDFe inválidas. Consulte os logs para detalhes.");
    }
}
else
{
    // Caso a seção "VasisMDFe" nem sequer exista ou não possa ser bindada
    var serviceProvider = builder.Services.BuildServiceProvider();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError("FATAL: Seção 'VasisMDFe' não encontrada ou inválida no 'appsettings.json'. A aplicação não pode ser iniciada sem estas configurações.");
    throw new InvalidOperationException("Falha na inicialização: Seção 'VasisMDFe' ausente ou inválida.");
}
// ===================================================================================
// FIM: Configuração do MDF-e
// ===================================================================================


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