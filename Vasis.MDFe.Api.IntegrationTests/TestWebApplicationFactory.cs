using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.Sources.Clear();

                // Múltiplas tentativas de localização do arquivo
                var possiblePaths = new[]
                {
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appsettings.Testing.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Testing.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.Testing.json")
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
                    conf.AddJsonFile(foundPath, optional: false, reloadOnChange: true);
                }
                else
                {
                    // Se não encontrar, criar configuração em memória
                    var inMemorySettings = new Dictionary<string, string>
                    {
                        ["JwtSettings:Key"] = "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres",
                        ["JwtSettings:Issuer"] = "VasisDfe.Testing",
                        ["JwtSettings:Audience"] = "VasisDfe.Testing.Client",
                        ["JwtSettings:ExpiryInMinutes"] = "60",
                        ["Logging:LogLevel:Default"] = "Information",
                        ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",
                        ["AllowedHosts"] = "*"
                    };
                    conf.AddInMemoryCollection(inMemorySettings);
                }

                conf.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Configurações específicas para testes podem ser adicionadas aqui
            });

            builder.UseEnvironment("Testing");
        }
    }
}