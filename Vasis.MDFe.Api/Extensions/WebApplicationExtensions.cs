// C:\\\\\\Zeus\\\\\\1935\\\\\\DFe.NET-2026\\\\\\Vasis.MDFe.Api.Extensions\\\\\\WebApplicationExtensions.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting; // ✅ ESTE USING É CRÍTICO para IsDevelopment e IsEnvironment
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using Microsoft.AspNetCore.Http; // Para HttpContext.Response.WriteAsync
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app)
        {
            // Ative o Swagger apenas em ambientes de desenvolvimento ou teste para não expor em produção.
            //if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
            //        c.RoutePrefix = "swagger"; // Acessível em /swagger
            //    });
            //}


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
                c.RoutePrefix = "swagger"; // Acessível em /swagger
            });

            var disableHttpsRedirectionForTests = app.Configuration.GetValue<bool>("DisableHttpsRedirectionForTests");

            if (!app.Environment.IsEnvironment("Testing") && !disableHttpsRedirectionForTests)
            {
                app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS em ambientes que não são de teste.
            }

            app.UseCors("AllowAll"); // Habilita o CORS.

            // ✅ CRÍTICO: UseRouting DEVE VIR ANTES de UseAuthentication e UseAuthorization.
            // Se esta ordem não for seguida, as rotas protegidas podem receber 401 Unauthorized
            // mesmo com um token válido, pois o sistema de autorização não saberá qual rota está sendo acessada.
            app.UseRouting();
            app.UseAuthentication(); // Deve vir antes de UseAuthorization.
            app.UseAuthorization();  // Deve vir antes de UseEndpoints para que as políticas sejam aplicadas.

            // ✅ UseEndpoints é o local correto para mapear controladores e outros endpoints
            // quando UseRouting é usado.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Mapeia os controladores da sua API.

                // Health Check Endpoint - Deixado como está, pois o foco não é consertá-lo agora.
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = async (context, report) =>
                    {
                        var customServicesStatus = new Dictionary<string, string>();
                        foreach (var entry in report.Entries)
                        {
                            string serviceName = entry.Key.Replace(" ", "");
                            customServicesStatus[serviceName] = entry.Value.Status.ToString(); // Usar o status REAL da API
                        }

                        var result = JsonSerializer.Serialize(new
                        {
                            Status = report.Status.ToString(),
                            Version = "1.0.0",
                            Timestamp = DateTime.UtcNow,
                            Services = customServicesStatus
                        }, new JsonSerializerOptions { WriteIndented = true });

                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
            });

            return app;
        }
    }
}