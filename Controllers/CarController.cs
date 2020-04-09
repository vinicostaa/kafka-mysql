using System.Text.Json;
using Kafka.Mysql.Example.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kafka.Mysql.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> _logger;
        private readonly IMemoryCache _cache;

        public CarController(ILogger<CarController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("{id}")]
        public ActionResult<CarCacheViewModel> Get(int id)
        {
            if (!_cache.TryGetValue(id, out var car))
            {
                _logger.LogError($"Product Id {id} not found");
                return NotFound($"Product Id {id} not found");
            }

            _logger.LogInformation(JsonSerializer.Serialize(car));

            return (CarCacheViewModel) car;
        }
    }
}
