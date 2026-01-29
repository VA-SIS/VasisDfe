using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly IConfiguration _configuration;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Test.json", optional: false);
                });
            });

            _client = _factory.CreateClient();

            // Configuração de teste
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: false);
            _configuration = configBuilder.Build();
        }

        /// <summary>
        /// Realiza login e retorna o token JWT para autenticação
        /// </summary>
        protected async Task<string> GetAuthTokenAsync()
        {
            var loginRequest = new
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            var loginContent = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Auth/login", loginContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);

            return result.token;
        }

        /// <summary>
        /// Configura o header de autorização com o token JWT
        /// </summary>
        protected void SetAuthorizationHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Helper para criar conteúdo JSON
        /// </summary>
        protected StringContent CreateJsonContent(object obj)
        {
            return new StringContent(
                JsonConvert.SerializeObject(obj),
                Encoding.UTF8,
                "application/json");
        }
    }
}