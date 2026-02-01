using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq; // Necessário para o método .Clear() de IServiceCollection, se for usar services.RemoveAll<T>

// O namespace do seu projeto de testes de integração.
// É crucial que seja o namespace correto do projeto Vasis.MDFe.Api.IntegrationTests.
namespace Vasis.MDFe.Api.IntegrationTests
{
    // A classe que representa o ponto de entrada da sua API é 'Program'
    // devido ao uso de top-level statements no .NET 6+
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Define o ambiente para "Testing".
            // Isso pode ser usado para carregar appsettings.Testing.json automaticamente
            // ou para ativar/desativar funcionalidades específicas de teste na API.
            builder.UseEnvironment("Testing");

            // Configura como a API irá carregar suas configurações *dentro do ambiente de teste*.
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                // Limpa todas as fontes de configuração padrão (como appsettings.json do projeto da API)
                // Isso nos dá controle total sobre quais configurações a API usará nos testes.
                conf.Sources.Clear();

                // 1. Carrega o appsettings.json padrão do projeto de testes (se houver, como fallback).
                //    Geralmente, não há um appsettings.json no projeto de testes, mas deixamos opcional.
                conf.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                // 2. Carrega OBRIGATORIAMENTE o appsettings.Testing.json.
                //    Este arquivo conterá as configurações JWT específicas para o ambiente de teste.
                //    Se este arquivo não for encontrado, a API não iniciará, forçando a presença das configurações.
                conf.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);

                // 3. Permite que variáveis de ambiente sobrescrevam as configurações (boa prática).
                conf.AddEnvironmentVariables();
            });

            // Opcional: Configurar serviços da API para usar Mocks/Fakes durante os testes.
            // Isso é útil se você quiser isolar partes da sua API para testar apenas o pipeline HTTP
            // ou se uma dependência externa for muito lenta/cara para ser usada em testes.
            builder.ConfigureServices(services =>
            {
                // Exemplo: Remover um serviço real e adicionar um mock
                // services.RemoveAll<IMDFeService>();
                // services.AddSingleton<IMDFeService, MockMDFeService>();
            });
        }
    }
}