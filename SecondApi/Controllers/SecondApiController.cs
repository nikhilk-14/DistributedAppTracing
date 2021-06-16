using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SecondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecondApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string thirdapi = "https://tracingthirdapi.azurewebsites.net";
        //private static readonly string thirdapi = "https://localhost:5003";
        private static readonly string function = "https://tracingfirstfunction.azurewebsites.net";
        //private static readonly string function = "http://localhost:7071";
        private static readonly string code = "?code=11aYevzeeW562j6wc8etJaFqEQMgKS5O0SvrJxVFOuZBK/IAEN29Hw==";
        private static readonly string connectionString = "Endpoint=sb://seyc-cms-api-rules-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=eix+eWEwG312aYnyjHbTvpBA3MV4Ah+onfr1VkSDm74=;EntityPath=";

        private readonly ILogger<SecondApiController> _logger;

        public SecondApiController(ILogger<SecondApiController> logger)
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
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();
            var traceId = this.HttpContext.TraceIdentifier;
            var connectionId = this.HttpContext.Connection.Id;

            _logger.LogInformation($"Start Second API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

            //var rng = new Random();
            //var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();

            IEnumerable<WeatherForecast> result = null;

            using (var _httpClient = new HttpClient())
            {
                var jsonStream = await
                      _httpClient.GetStreamAsync($"{thirdapi}/thirdapi");
                result = await
                  System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(jsonStream);
            }

            _logger.LogInformation($"Finish Second API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return result;
        }

        [HttpGet]
        [Route("queue")]
        public async Task<string> Queue()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();

            _logger.LogInformation($"Start Second API - queue (RequestId - {requestId}): {DateTime.Now.ToString()}");

            var queueInput = "{\"Test\":\"Sample\",\"RequestId\":\"" + requestId + "\"}";

            await GenerateAndSaveQueueMessage($"{connectionString}seyc-tracing-test", queueInput);

            _logger.LogInformation($"Finish Second API - queue (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return requestId;
        }

        [HttpGet]
        [Route("httptrigger")]
        public async Task<string> HttpTrigger()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();

            _logger.LogInformation($"Start Second API - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            using (var _httpClient = new HttpClient())
            {
                await _httpClient.GetStringAsync($"{function}/api/function1{code}");
            }

            _logger.LogInformation($"Finish Second API - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return requestId;
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

        public async Task GenerateAndSaveQueueMessage<T>(string connectionString, T input)
        {
            var serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            var client = new QueueClient(serviceBusConnectionStringBuilder);
            string jsonMessage = JsonConvert.SerializeObject(input);
            var message = new Message(Encoding.UTF8.GetBytes(jsonMessage));
            await client.SendAsync(message);
            await client.CloseAsync();
        }
    }
}
