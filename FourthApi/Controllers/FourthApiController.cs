using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace FourthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FourthApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<FourthApiController> _logger;

        public FourthApiController(ILogger<FourthApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get_data")]
        public void GetData()
        {
            var operationId = System.Diagnostics.Activity.Current.RootId;
            _logger.LogInformation($"FourthApiController OperationId => {operationId}");

            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            _logger.LogInformation($"FourthApiController Result: {result}");
        }
    }
}
