using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ThirdApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThirdApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ThirdApiController> _logger;

        public ThirdApiController(ILogger<ThirdApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("test")]
        public IEnumerable<WeatherForecast> Test()
        {
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return result;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();

            _logger.LogInformation($"Start Third API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation($"Start Third API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return result;
        }

        [HttpGet]
        [Route("signalr")]
        public IEnumerable<WeatherForecast> SignalR()
        {
            var operationId = System.Diagnostics.Activity.Current.RootId;
            _logger.LogInformation($"signalr Controller OperationId => {operationId}");

            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return result;
        }
    }
}
