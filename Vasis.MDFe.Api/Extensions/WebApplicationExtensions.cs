using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
                c.RoutePrefix = "swagger";
            });

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

            return app;
        }
    }
}