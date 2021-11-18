using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenFoodFactService.Services;

namespace OpenFoodFactService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenFoodFactController : ControllerBase
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<OpenFoodFactController> _logger;

        public OpenFoodFactController(
            ILogger<OpenFoodFactController> logger,
            IApiClient apiClient
        )
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Get(string id)
        {
            var result = _apiClient.ConnectToApi(id);
            
            return Ok(result);
        }
    }
}