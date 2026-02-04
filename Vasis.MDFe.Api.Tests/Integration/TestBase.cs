using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Xunit;

namespace Vasis.MDFe.Api.Tests.Integration
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
    }
}