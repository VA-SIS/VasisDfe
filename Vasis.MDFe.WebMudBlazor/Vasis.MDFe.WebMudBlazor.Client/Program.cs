using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http.Json; // Usado para GetFromJsonAsync
using Vasis.MDFe.Common.Models; // DTOs compartilhados com a API
using Vasis.MDFe.WebMudBlazor.Client.Services; // Para o MdfeConfigApiClient
using Microsoft.Extensions.Http; // Necessário para AddHttpClient

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configuração do HttpClient padrão para o BaseAddress do próprio Blazor App
// Este é o HttpClient que o Blazor usa para recursos estáticos, etc.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configuração do MudBlazor
builder.Services.AddMudServices();

// ==================================================================================================
// Adição do HttpClient tipado para a comunicação com a Vasis.MDFe.Api
// ==================================================================================================
builder.Services.AddHttpClient<MdfeConfigApiClient>(client =>
{
    // !!! IMPORTANTE: AJUSTE ESTA URL PARA O ENDEREÇO DA SUA API Vasis.MDFe.Api !!!
    // Exemplo: "https://localhost:7001" (se sua API estiver rodando nesta porta)
    client.BaseAddress = new Uri("https://localhost:7001"); // <--- AJUSTE ESTA URL!
});
// ==================================================================================================

await builder.Build().RunAsync();