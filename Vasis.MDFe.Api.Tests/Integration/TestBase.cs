using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Xunit; // Importante para IClassFixture

namespace Vasis.MDFe.Api.Tests.Integration // ✅ Este é o namespace correto
{
    public abstract class TestBase : IClassFixture<TestWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected readonly TestWebApplicationFactory Factory;

        protected TestBase(TestWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        protected T GetService<T>() where T : notnull
        {
            using var scope = Factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}