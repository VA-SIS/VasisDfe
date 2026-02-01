// File: Vasis.MDFe.Api.IntegrationTests\TestWebApplicationFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Usado para IHostEnvironment
using System; // Usado para AppDomain
using System.Collections.Generic;
using System.IO;
using System.Reflection; // Usado para Assembly

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Define o ambiente da aplicação para "Testing" ANTES de configurar o builder.
            // Isso garante que o pipeline da aplicação seja configurado para testes,
            // e que o IConfiguration carregue as variáveis de ambiente correspondentes.
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                // **IMPORTANTE**: Não usar conf.Sources.Clear()!
                // Ao remover o conf.Sources.Clear(), a WebApplicationFactory carregará
                // a configuração padrão da API (appsettings.json, appsettings.Development.json, etc.).
                // Nosso appsettings.Testing.json será adicionado por último, permitindo que
                // ele sobrescreva APENAS as configurações específicas de teste, mantendo o restante.

                // Localiza o appsettings.Testing.json. A ordem é importante aqui.
                // A primeira opção (baseada no assembly de testes) é a mais robusta para WebApplicationFactory.
                var integrationTestsAssembly = Assembly.GetExecutingAssembly();
                var configFileName = "appsettings.Testing.json";
                var configPath = Path.Combine(Path.GetDirectoryName(integrationTestsAssembly.Location), configFileName);

                string foundPath = null;
                // Busca o arquivo em alguns locais comuns do projeto de testes
                var possiblePaths = new[]
                {
                    Path.Combine(Path.GetDirectoryName(integrationTestsAssembly.Location), configFileName), // Próximo ao DLL dos testes
                    Path.Combine(Directory.GetCurrentDirectory(), configFileName), // Diretório de execução do teste
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName) // Base da AppDomain
                };

                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    // Adiciona o appsettings.Testing.json. Ele sobrescreverá as configurações existentes.
                    // 'optional: false' garantirá que o teste falhe se este arquivo crucial não for encontrado.
                    conf.AddJsonFile(foundPath, optional: false, reloadOnChange: false); // reloadOnChange: false para testes
                    Console.WriteLine($"[TestWebApplicationFactory] Loaded config from: {foundPath}");
                }
                else
                {
                    // Se o appsettings.Testing.json não for encontrado, cria uma configuração JWT em memória.
                    // Os valores aqui DEVEM ser consistentes com o que sua API real espera validar (VasisDfe.Api).
                    var inMemorySettings = new Dictionary<string, string>
                    {
                        ["JwtSettings:Key"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres",
                        ["JwtSettings:Issuer"] = "VasisDfe.Api", // Consistente com appsettings.json da API
                        ["JwtSettings:Audience"] = "VasisDfe.Api.Clients", // Consistente com appsettings.json da API
                        ["JwtSettings:ExpiryInMinutes"] = "60",
                        ["Logging:LogLevel:Default"] = "Information",
                        ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                        ["AllowedHosts"] = "*"
                    };
                    conf.AddInMemoryCollection(inMemorySettings);
                    Console.WriteLine("[TestWebApplicationFactory] WARNING: appsettings.Testing.json not found for integration tests. Using in-memory fallback settings for JWT configuration.");
                }

                // Adiciona variáveis de ambiente para que possam sobrescrever qualquer coisa,
                // mantendo a precedência padrão do .NET Core.
                conf.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Aqui você pode adicionar ou substituir serviços para o ambiente de teste.
                // Por exemplo, mockar um serviço de banco de dados, ou um serviço externo.
                // services.RemoveAll<SomeService>();
                // services.AddSingleton<SomeService, MockSomeService>();

                // Se houver algum hosted service (IHostedService) que você não quer que rode nos testes,
                // você pode removê-lo aqui:
                // var hostedServices = services.Where(s => s.ServiceType == typeof(IHostedService)).ToList();
                // foreach (var hostedService in hostedServices)
                // {
                //     services.Remove(hostedService);
                // }
            });

            // O ambiente "Testing" já foi definido no início de ConfigureWebHost.
            // Para depuração adicional, você pode adicionar logging para ver as configurações sendo carregadas:
            // builder.ConfigureLogging(loggingBuilder =>
            // {
            //     loggingBuilder.ClearProviders(); // Limpa provedores de log padrão
            //     loggingBuilder.AddConsole(); // Adiciona console logger
            // });
        }
    }
}