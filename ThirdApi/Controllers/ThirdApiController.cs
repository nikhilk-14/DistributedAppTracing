using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [Route("get_data")]
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
