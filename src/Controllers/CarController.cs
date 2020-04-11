using System.Text.Json;
using Kafka.Mysql.Example.Interfaces.Services;
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
        private readonly IRepositoryService _repositoryService;

        public CarController(ILogger<CarController> logger,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _repositoryService = repositoryService;
        }

        [HttpGet("{id}")]
        public ActionResult<CarCacheViewModel> Get(int id)
        {
            if (!_repositoryService.GetByIdFromCache(id, out var car))
            {
                _logger.LogError($"Car Id {id} not found");
                return NotFound($"Car Id {id} not found");
            }

            _logger.LogInformation(JsonSerializer.Serialize(car));
            return (CarCacheViewModel) car;
        }
    }
}
