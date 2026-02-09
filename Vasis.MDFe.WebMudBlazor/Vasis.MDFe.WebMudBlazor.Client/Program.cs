using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http.Json; // Usado para GetFromJsonAsync
using Vasis.MDFe.Common.Models; // DTOs compartilhados com a API
using Vasis.MDFe.WebMudBlazor.Client.Services; // Para o MdfeConfigApiClient
using Microsoft.Extensions.Http; // Necessário para AddHttpClient, pacote Microsoft.Extensions.Http

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configuração padrão do HttpClient para o BaseAddress do próprio Blazor App
// Este HttpClient é usado para recursos estáticos do próprio Blazor.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configuração do MudBlazor
builder.Services.AddMudServices();

// ==================================================================================================
// Adição do HttpClient tipado para a comunicação com a Vasis.MDFe.Api
// ==================================================================================================

// Carrega appsettings.json e appsettings.{Environment}.json para ler a URL da API
// Este `using` builder.Configuration.AddJsonFile é importante, mas o Blazor WebAssembly
// já injeta o IConfiguration que lê appsettings.json se ele existir na pasta wwwroot.
// Então, o que faremos é ler direto do builder.Configuration
// Se você não tiver um appsettings.json na wwwroot e quiser usar apenas as Application Settings
// do Azure, não precisa dessas linhas, apenas da leitura via builder.Configuration["ApiBaseUrl"].

// Note: No Blazor WASM, a configuração é lida de wwwroot/appsettings.json.
// Para configurar dinamicamente em produção (Azure), você pode usar o Azure Static Web Apps
// ou passar variáveis de ambiente para o JavaScript do Blazor, o que é um pouco mais complexo.
// Por ora, vamos simplificar lendo do `builder.Configuration` e garantindo que haja um fallback.

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    // Fallback para um valor padrão se 'ApiBaseUrl' não for encontrado
    // ou se estiver rodando localmente sem appsettings.json em wwwroot.
    // Lembre-se de ajustar este URL para o da sua API local!
    apiBaseUrl = "https://localhost:7001"; // <--- AJUSTE ESTA URL LOCAL DE DEFAULT SE NECESSÁRIO
    Console.WriteLine($"Aviso: 'ApiBaseUrl' não configurada explicitamente. Usando fallback: {apiBaseUrl}");
}


builder.Services.AddHttpClient<MdfeConfigApiClient>(client =>
{
    // !!! IMPORTANTE: AJUSTE ESTA URL PARA O ENDEREÇO DA SUA API Vasis.MDFe.Api !!!
    // Se você já tem um appsettings.json em wwwroot, o valor 'apiBaseUrl' já virá de lá.
    // Caso contrário, ele usará o fallback definido acima.
    client.BaseAddress = new Uri(apiBaseUrl);
});
// ==================================================================================================

await builder.Build().RunAsync();