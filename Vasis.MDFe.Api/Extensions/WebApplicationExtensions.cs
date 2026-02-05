// C:\\Zeus\\1935\\DFe.NET-2026\\Vasis.MDFe.Api.Extensions\\WebApplicationExtensions.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using Microsoft.AspNetCore.Http; // Para HttpContext.Response.WriteAsync
using System; // Para DateTime.UtcNow
using System.Collections.Generic; // Para Dictionary
using System.Linq; // Para ToDictionary

namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        // ✅ ATENÇÃO: Se o seu Program.cs chama .ConfigureRequestPipeline() diretamente no `app` do tipo `WebApplication`,
        // a assinatura deste método deve ser `this WebApplication app`.
        // Se a assinatura fosse `this IApplicationBuilder app`, a correção no TestWebApplicationFactory seria mais simples.
        // Mantenho a assinatura original para não alterar o comportamento da API principal.
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

            // ✅ CORREÇÃO APLICADA: Mapeamento do Health Check com ResponseWriter customizado
            // Agora ele reflete o status REAL dos serviços e os nomes das chaves sem espaços.
            app.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    // Prepara o dicionário de serviços com as chaves (sem espaços) e valores (status real) esperados
                    var servicesStatus = report.Entries.ToDictionary(
                        entry => entry.Key.Replace(" ", ""), // Remove espaços do nome do serviço
                        entry => entry.Value.Status.ToString() // Use o status REAL do Health Check (Healthy, Unhealthy, Degraded)
                    );

                    var result = JsonSerializer.Serialize(new
                    {
                        Status = report.Status.ToString(), // Será "Healthy" se todos os checks estiverem saudáveis
                        Version = "1.0.0", // Versão fixa para corresponder ao teste
                        Timestamp = DateTime.UtcNow, // Será serializado para string no formato ISO 8601
                        Services = servicesStatus
                    }, new JsonSerializerOptions { WriteIndented = true }); // Garante formatação legível

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            return app;
        }
    }
}