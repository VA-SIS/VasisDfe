// C:\Zeus\1935\DFe.NET-2026\Vasis.MDFe.Api.Tests\Integration\TestWebApplicationFactory.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO; // Necessário para Path.Combine e Directory.GetCurrentDirectory
using Vasis.MDFe.Api.Extensions; // Para garantir que os métodos de extensão da API estejam disponíveis (se necessário)

namespace Vasis.MDFe.Api.Tests.Integration
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Limpa as fontes de configuração existentes para ter controle total no ambiente de teste.
                // Esta linha pode ser comentada se você precisar que algumas configurações padrão sejam carregadas.
                // config.Sources.Clear();

                // Adiciona os arquivos de configuração diretamente do projeto da API.
                // Isto é crucial, pois WebApplicationFactory pode não carregar automaticamente
                // o appsettings.json do projeto de API quando o ConfigureWebHost é sobrescrito.
                var apiProjectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Vasis.MDFe.Api");
                config.AddJsonFile(Path.Combine(apiProjectDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
                      .AddJsonFile(Path.Combine(apiProjectDirectory, "appsettings.Testing.json"), optional: true, reloadOnChange: true);

                // Adiciona configurações em memória que sobrescrevem ou complementam as do appsettings.
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres",
                    ["Jwt:Issuer"] = "Vasis.MDFe.Api",
                    ["Jwt:Audience"] = "aplicacoes_clientes",
                    ["DisableHttpsRedirectionForTests"] = "true",
                    ["Logging:LogLevel:Default"] = "Warning",
                    ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                    ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Warning"
                });
            });

            // ✅ CORREÇÃO CRÍTICA: ConfigureTestServices é o local ideal para registrar ou sobrescrever serviços
            // ESPECIFICAMENTE para o ambiente de teste, após a configuração normal da aplicação.
            builder.ConfigureTestServices(services =>
            {
                // **Garante que AddHealthChecks() seja chamado e os serviços de Health Checks estejam disponíveis.**
                // Isso resolve a "System.InvalidOperationException: Unable to find the required services.
                // Please add all the required services by calling 'IServiceCollection.AddHealthChecks'"
                services.AddHealthChecks();

                // IMPORTANTE:
                // Embora você possa chamar seus métodos de extensão da API aqui (ex: services.AddCoreServices(context.Configuration);),
                // o WebApplicationFactory já tenta carregar e configurar o Program.cs da sua API.
                // Chamar AddHealthChecks() diretamente em ConfigureTestServices é uma maneira robusta de garantir
                // que este serviço esteja presente no contexto de teste, sem duplicar a configuração completa da API.
                // Se seus outros métodos de extensão da API contêm lógica *essencial e não duplicável* para o
                // funcionamento dos testes, eles TAMBÉM deveriam ser chamados aqui, como:
                // services.AddCoreServices(context.Configuration);
                // services.AddJwtAuthentication(context.Configuration);
                // No entanto, para o erro atual, o foco está no HealthChecks.
            });

            // Configuração do logger para os testes
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Limpa os providers de log padrão
                logging.AddConsole();     // Adiciona apenas o console para ver os logs dos testes
                logging.SetMinimumLevel(LogLevel.Warning); // Define o nível de log para Warning
            });
        }
    }
}