using Microsoft.AspNetCore.Mvc;

namespace Vasis.MDFe.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Endpoint de verificação de saúde da aplicação
        /// </summary>
        /// <returns>Status da aplicação e seus serviços</returns>
        [HttpGet]
        public IActionResult GetHealth()
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Services = new
                {
                    ValidationService = "Active",
                    LifecycleService = "Active"
                }
            };

            return Ok(healthStatus);
        }
    }
}