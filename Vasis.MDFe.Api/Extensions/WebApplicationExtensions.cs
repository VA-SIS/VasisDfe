namespace Vasis.MDFe.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vasis MDFe API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
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