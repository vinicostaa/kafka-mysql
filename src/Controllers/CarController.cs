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
        private readonly IMemoryCache _cache;
        private readonly IRepositoryService _repositoryService;

        public CarController(ILogger<CarController> logger, IMemoryCache cache,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _cache = cache;
            _repositoryService = repositoryService;
        }

        //{"id":1,"name":"Fusca","color":"Amarelo","price":20,"creation_time":1586535653000,"modification_time":1586540515000}

        [HttpGet("{id}")]
        public ActionResult<CarCacheViewModel> Get(int id)
        {
            //if (!_cache.TryGetValue(id, out var car))
            //{
            //    _logger.LogError($"Car Id {id} not found");
            //    return NotFound($"Car Id {id} not found");
            //}

            _repositoryService.GetByIdFromCache(id, out var car);
            _logger.LogInformation(JsonSerializer.Serialize(car));

            return (CarCacheViewModel) car;
        }

        //[HttpPost]
        //public IActionResult Post([FromBody] CarCacheViewModel car)
        //{
        //    _repositoryService.Set(car);
        //    Ok();
        //}
    }
}
