// C:\\\\\\Zeus\\\\\\1935\\\\\\DFe.NET-2026\\\\\\Vasis.MDFe.Api.Tests\\\\\\Integration\\\\\\TestWebApplicationFactory.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

// NENHUM USING EXTRA É NECESSÁRIO AQUI, pois não tentaremos mais redefinir o pipeline ou serviços que já são definidos no Program.cs.

namespace Vasis.MDFe.Api.Tests.Integration
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var apiProjectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Vasis.MDFe.Api");
                config.AddJsonFile(Path.Combine(apiProjectDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
                      .AddJsonFile(Path.Combine(apiProjectDirectory, "appsettings.Testing.json"), optional: true, reloadOnChange: true);

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

            // ✅ CRÍTICO: Removido QUALQUER chamada a services.AddJwtAuthentication() aqui.
            // O WebApplicationFactory carregará o Program.cs da sua API, que já configura isso uma vez.
            // ConfigureTestServices será usado APENAS se você precisar *substituir*
            // um serviço já existente na API por um mock, mas não para adicionar serviços duplicados.
            // Por enquanto, não há necessidade de sobrescrever nada aqui para os testes Auth/MDFe.

            // ✅ Removido o bloco builder.Configure(app => { ... }) COMPLETAMENTE.
            // O WebApplicationFactory usa o Program.cs da sua API para construir o pipeline.

            // Configuração do logger para os testes
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        }
    }
}