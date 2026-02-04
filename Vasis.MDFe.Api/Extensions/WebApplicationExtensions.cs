using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app)
        {
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
            //        c.RoutePrefix = "swagger";
            //    });
            //}

            // ***** ALTERAÇÃO AQUI: Habilitar o Swagger para todos os ambientes *****

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
                c.RoutePrefix = "swagger";
            });





            // ***** CORREÇÃO AQUI: Desabilitar o redirecionamento HTTPS para o ambiente de testes *****
            // Verifica se a flag "DisableHttpsRedirectionForTests" está definida e é verdadeira
            var disableHttpsRedirectionForTests = app.Configuration.GetValue<bool>("DisableHttpsRedirectionForTests");

            // Só usa o redirecionamento HTTPS se não for o ambiente "Testing" E se a flag não estiver ativada
            if (!app.Environment.IsEnvironment("Testing") && !disableHttpsRedirectionForTests)
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapGet("/health", () => new {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Services = new
                {
                    ValidationService = "Active",
                    LifecycleService = "Active"
                }
            });

            return app;
        }
    }
}