using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting; // Para IWebHostEnvironment (IsDevelopment)

namespace Vasis.MDFe.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureRequestPipeline(this WebApplication app)
        {
            // O UseCustomSwagger já verifica o ambiente.
            // Aqui chamamos apenas a parte que configura a interface do Swagger.
            app.UseCustomSwagger(app.Environment);

            // Configurações para ambiente de Desenvolvimento E TESTE (se não for tratada no Swagger)
            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Redirecionamento HTTPS
            app.UseHttpsRedirection();

            // --- ORDEM CORRETA DOS MIDDLEWARES JWT ---
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowSwaggerUI"); // Usa o nome da política definida em ServiceCollectionExtensions

            // Mapeia os controllers da aplicação
            app.MapControllers();

            return app;
        }
    }
}