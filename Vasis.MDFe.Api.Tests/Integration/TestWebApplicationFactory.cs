// C:\\\\\\Zeus\\\\\\1935\\\\\\DFe.NET-2026\\\\\\Vasis.MDFe.Api.Tests\\\\\\Integration\\\\\\TestWebApplicationFactory.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

// Importe apenas o que é estritamente necessário para TestWebApplicationFactory
using Vasis.MDFe.Api.Extensions; // Necessário para chamar AddJwtAuthentication no ConfigureTestServices

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

            // ✅ ConfigureTestServices: Este é o local onde você pode sobrescrever ou adicionar
            // serviços *após* o Program.cs da API ter registrado seus serviços.
            // É crucial garantir que a autenticação JWT esteja configurada no ambiente de teste.
            builder.ConfigureTestServices(services =>
            {
                // Garante que a autenticação JWT esteja configurada para os testes.
                // Isso resolve o 401 Unauthorized nos testes do MDFeController quando o token é válido.
                services.AddJwtAuthentication(services.BuildServiceProvider().GetRequiredService<IConfiguration>());

                // IMPORTANTE: Não faremos mock ou sobrescreveremos os Health Checks aqui,
                // pois você decidiu focar nos testes Auth e MDFe por enquanto.
                // O WebApplicationFactory executará o Health Check real da sua API.
            });

            // ✅ MUITO IMPORTANTE: O bloco builder.Configure(app => { ... }) FOI REMOVIDO COMPLETAMENTE.
            // O WebApplicationFactory vai usar o Program.cs da sua API para construir o pipeline real da aplicação.
            // Isso evita os erros de compilação anteriores.

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