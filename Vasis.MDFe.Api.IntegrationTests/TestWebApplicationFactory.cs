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
                conf.Sources.Clear();

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

                conf.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);

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
                    // Assegure-se de que 'JwtSettings' aqui corresponde ao que sua API usa.
                    ["JwtSettings:SecretKey"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres-e-eh-longa",
                    ["JwtSettings:Issuer"] = "VasisDfe.Api",
                    ["JwtSettings:Audience"] = "VasisDfe.Api.Clients",
                    ["JwtSettings:ExpiryInMinutes"] = "60",

                    ["ZeusConfig:DiretorioSchemas"] = @"C:\Zeus\Schemas",
                    ["ZeusConfig:DiretorioTemplates"] = @"C:\Zeus\Templates",
                    ["ZeusConfig:DiretorioSaida"] = @"C:\Zeus\Output",
                    ["ZeusConfig:TipoAmbiente"] = "2", // Homologação
                    ["ZeusConfig:VersaoServico"] = "3.00",

                    ["Certificado:CaminhoArquivo"] = @"C:\Zeus\Certificados\certificado_teste.pfx",
                    ["Certificado:Senha"] = "123456",
                    ["Certificado:Serial"] = "",
                    ["Certificado:Thumbprint"] = "",

                    ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",

                    ["Logging:LogLevel:Default"] = "Information",
                    ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                    ["Logging:LogLevel:Vasis.MDFe.Api"] = "Debug",
                    ["AllowedHosts"] = "*",

                    ["DisableHttpsRedirectionForTests"] = "true"
                };

                conf.AddInMemoryCollection(inMemorySettings);
                conf.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Configurações específicas para testes se necessário.
            });
        }
    }
}