using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                // Localiza o appsettings.Testing.json
                var integrationTestsAssembly = Assembly.GetExecutingAssembly();
                var configFileName = "appsettings.Testing.json";

                var possiblePaths = new[]
                {
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

                if (foundPath != null)
                {
                    conf.AddJsonFile(foundPath, optional: false, reloadOnChange: false);
                    Console.WriteLine($"[TestWebApplicationFactory] Loaded config from: {foundPath}");
                }
                else
                {
                    // ✅ CORRIGIDO: Usando @ para verbatim strings
                    var inMemorySettings = new Dictionary<string, string>
                    {
                        ["JwtSettings:SecretKey"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres-e-eh-longa",
                        ["JwtSettings:Issuer"] = "VasisDfe.Api",
                        ["JwtSettings:Audience"] = "VasisDfe.Api.Clients",
                        ["JwtSettings:ExpiryMinutes"] = "60",

                        // ✅ CORRIGIDO: Usando @ para paths do Windows
                        ["ZeusConfig:DiretorioSchemas"] = @"C:\Zeus\Schemas",
                        ["ZeusConfig:DiretorioTemplates"] = @"C:\Zeus\Templates",
                        ["ZeusConfig:DiretorioSaida"] = @"C:\Zeus\Output",
                        ["ZeusConfig:TipoAmbiente"] = "2",
                        ["ZeusConfig:VersaoServico"] = "3.00",

                        ["Certificado:CaminhoArquivo"] = @"C:\Zeus\Certificados\certificado_teste.pfx",
                        ["Certificado:Senha"] = "123456",
                        ["Certificado:Serial"] = "",
                        ["Certificado:Thumbprint"] = "",

                        ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",

                        ["Logging:LogLevel:Default"] = "Information",
                        ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                        ["Logging:LogLevel:Vasis.MDFe.Api"] = "Debug",
                        ["AllowedHosts"] = "*"
                    };
                    conf.AddInMemoryCollection(inMemorySettings);

                    var expectedPath = Path.Combine(Path.GetDirectoryName(integrationTestsAssembly.Location), configFileName);
                    throw new FileNotFoundException(
                        $"O arquivo de configuração '{configFileName}' NÃO FOI ENCONTRADO no caminho esperado: '{expectedPath}'. " +
                        "Por favor, verifique se o arquivo está na raiz do seu projeto de testes (Vasis.MDFe.Api.IntegrationTests) " +
                        "e se o .csproj está configurado para copiá-lo para o diretório de saída " +
                        "(com <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>).");
                }

                conf.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Configurações específicas para testes se necessário
            });
        }
    }
}