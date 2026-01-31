using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Vasis.MDFe.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomPipeline(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Usar Swagger em desenvolvimento
            app.UseCustomSwagger(env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Middleware de logging de requisições
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogDebug("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await next();
                logger.LogDebug("Response: {StatusCode}", context.Response.StatusCode);
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");

            // Ordem crítica: Authentication antes de Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}