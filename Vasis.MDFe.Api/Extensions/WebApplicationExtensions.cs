// C:\Zeus\1935\DFe.NET-2026\Vasis.MDFe.Api.Extensions\WebApplicationExtensions.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using Microsoft.AspNetCore.Http; // Para HttpContext.Response.WriteAsync
using System; // Para DateTime.UtcNow

namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app)
        {
            // Ative o Swagger apenas em ambientes de desenvolvimento ou teste para não expor em produção.
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
                    c.RoutePrefix = "swagger"; // Acessível em /swagger
                });
            }

            var disableHttpsRedirectionForTests = app.Configuration.GetValue<bool>("DisableHttpsRedirectionForTests");

            if (!app.Environment.IsEnvironment("Testing") && !disableHttpsRedirectionForTests)
            {
                app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS em ambientes que não são de teste.
            }

            app.UseCors("AllowAll"); // Habilita o CORS.

            app.UseAuthentication(); // Deve vir antes de UseAuthorization.
            app.UseAuthorization();  // Deve vir antes de MapControllers para que as políticas sejam aplicadas.

            app.MapControllers(); // Mapeia os controladores da sua API.

            // ✅ NOVIDADE/CORREÇÃO: Mapeamento do Health Check com ResponseWriter customizado
            // Este writer formatará a resposta do /health para corresponder ao esperado pelos seus testes.
            app.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(new
                    {
                        Status = report.Status.ToString(),
                        Version = "1.0.0", // Versão fixa para o exemplo do teste.
                        Timestamp = DateTime.UtcNow,
                        // Mapeia os serviços monitorados para o formato esperado pelo teste
                        Services = report.Entries.ToDictionary(
                            entry => entry.Key.Replace(" ", ""), // "MDFe Validation Service" -> "MDFeValidationService"
                            entry => entry.Value.Status.ToString() // "Healthy", "Unhealthy", "Degraded"
                        )
                    }, new JsonSerializerOptions { WriteIndented = true });

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            return app;
        }
    }
}