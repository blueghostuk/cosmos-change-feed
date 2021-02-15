using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastRepository _repository;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(WeatherForecastRepository repository, ILogger<WeatherForecastController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
        {
            return await this._repository.Get(cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> Put(WeatherForecast model, CancellationToken cancellationToken)
        {
            var response = await this._repository.AddAsync(model, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(WeatherForecast model, CancellationToken cancellationToken)
        {
            await this._repository.UpdateAsync(model, cancellationToken);
            return Accepted();
        }
    }
}
