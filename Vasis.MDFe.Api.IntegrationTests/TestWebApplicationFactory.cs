using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.Sources.Clear(); // Limpa as fontes de configuração existentes para ter controle total

                var integrationTestsAssembly = Assembly.GetExecutingAssembly();
                var configFileName = "appsettings.Testing.json";

                var possiblePaths = new[]
                {
                    // Tenta encontrar appsettings.Testing.json em diferentes locais
                    Path.Combine(Path.GetDirectoryName(integrationTestsAssembly.Location), configFileName),
                    Path.Combine(Directory.GetCurrentDirectory(), configFileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName)
                };

                string foundPath = null;
                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                // Adiciona appsettings.json e appsettings.Development.json se existirem
                conf.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);

                // Adiciona o appsettings.Testing.json se encontrado
                if (foundPath != null)
                {
                    conf.AddJsonFile(foundPath, optional: false, reloadOnChange: false);
                    Console.WriteLine($"[TestWebApplicationFactory] Loaded config from: {foundPath}");
                }
                else
                {
                    Console.WriteLine($"[TestWebApplicationFactory] {configFileName} NOT FOUND in expected paths. Using in-memory settings only.");
                }

                var inMemorySettings = new Dictionary<string, string?>
                {
                    // ***** CORREÇÃO APLICADA AQUI: prefixo "Jwt:" conforme o AddJwtAuthentication *****
                    ["Jwt:Key"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres-e-eh-longa",
                    ["Jwt:Issuer"] = "VasisDfe.Api",
                    ["Jwt:Audience"] = "VasisDfe.Api.Clients",
                    ["Jwt:ExpiryInMinutes"] = "60", // Mantenha, se precisar para o teste de expiração

                    // Configurações para Zeus (DFe.NET)
                    ["ZeusConfig:DiretorioSchemas"] = @"C:\Zeus\Schemas",
                    ["ZeusConfig:DiretorioTemplates"] = @"C:\Zeus\Templates",
                    ["ZeusConfig:DiretorioSaida"] = @"C:\Zeus\Output",
                    ["ZeusConfig:TipoAmbiente"] = "2", // Homologação
                    ["ZeusConfig:VersaoServico"] = "3.00",

                    // Configurações de Certificado
                    ["Certificado:CaminhoArquivo"] = @"C:\Zeus\Certificados\certificado_teste.pfx",
                    ["Certificado:Senha"] = "123456",
                    ["Certificado:Serial"] = "", // Preencha se for relevante para os testes
                    ["Certificado:Thumbprint"] = "", // Preencha se for relevante para os testes

                    // Configurações de Banco de Dados (Exemplo: SQLite in-memory para testes)
                    ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",

                    // Configurações de Logging
                    ["Logging:LogLevel:Default"] = "Information",
                    ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                    ["Logging:LogLevel:Vasis.MDFe.Api"] = "Debug",
                    ["AllowedHosts"] = "*",

                    // Flag para desabilitar redirecionamento HTTPS em testes
                    ["DisableHttpsRedirectionForTests"] = "true"
                };

                // Adiciona configurações em memória, que têm precedência sobre os arquivos JSON
                conf.AddInMemoryCollection(inMemorySettings);
                // Permite sobrescrever com variáveis de ambiente (útil para CI/CD)
                conf.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Aqui você pode adicionar mocks ou substituir serviços para os testes de integração.
                // Exemplo: services.RemoveAll<ISomeService>();
                // services.AddSingleton<ISomeService, MockSomeService>();
            });
        }
    }
}