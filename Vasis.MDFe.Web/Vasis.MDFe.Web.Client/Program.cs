using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Vasis.MDFe.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configurar HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7001/")
});

// Registrar Services
builder.Services.AddScoped<MDFeService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<ConfiguracaoService>();

// MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();