using MudBlazor.Services;
using Vasis.MDFe.Web.Client.Pages;
using Vasis.MDFe.Web.Components;
using Vasis.MDFe.Web.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// 🚀 Configuração dos serviços do MudBlazor
builder.Services.AddMudServices();

// 🔧 Configuração dos componentes Razor com WebAssembly
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// 🌐 Configuração do HttpClient para consumo da API
builder.Services.AddHttpClient("MDFeAPI", client =>
{
    // 🔥 Substitua pela URL da sua API
    client.BaseAddress = new Uri("https://localhost:7001/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// 📡 Registrar HttpClient padrão para injeção de dependência
builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("MDFeAPI");
});

// 🔥 Registrar serviços da aplicação
builder.Services.AddScoped<MDFeService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<ConfiguracaoService>();

// 📋 Configurações adicionais
builder.Services.AddLogging();

var app = builder.Build();

// 🛠️ Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // 🔒 HSTS para produção - 30 dias padrão
    app.UseHsts();
}

// 🔐 Redirecionamento HTTPS
app.UseHttpsRedirection();

// 📁 Arquivos estáticos
app.UseStaticFiles();

// 🛡️ Proteção contra CSRF
app.UseAntiforgery();

// 🗺️ Mapeamento dos componentes Razor
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Vasis.MDFe.Web.Client._Imports).Assembly);

// 🚀 Executar a aplicação
app.Run();