using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirstApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string secondapi = "https://tracingsecondapi.azurewebsites.net";
        //private static readonly string secondapi = "https://localhost:5002";
        private static readonly string thirdapi = "https://tracingthirdapi.azurewebsites.net";
        //private static readonly string thirdapi = "https://localhost:5003";
        private static readonly string function = "https://tracingfirstfunction.azurewebsites.net";
        //private static readonly string function = "http://localhost:7071";
        private static readonly string code = "?code=11aYevzeeW562j6wc8etJaFqEQMgKS5O0SvrJxVFOuZBK/IAEN29Hw==";
        private static readonly string connectionString = "Endpoint=sb://seyc-cms-api-rules-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2e/0GFhqroj895yo0Ky6x00UFZdCXe17m0BYkgjfZII=;EntityPath=";

        private readonly ILogger<FirstApiController> _logger;
        //private readonly HttpClient _httpClient;

        public FirstApiController(ILogger<FirstApiController> logger)//, HttpClient httpClient)
        {
            _logger = logger;
            //_httpClient = httpClient;
        }

        [HttpGet]
        [Route("test")]
        public IEnumerable<WeatherForecast> Test()
        {
            var operationId = System.Diagnostics.Activity.Current.RootId;
            _logger.LogInformation($"Controller OperationId => {operationId}");

            var traceId = this.HttpContext.TraceIdentifier;
            _logger.LogInformation($"Controller TraceId => {traceId}");

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

            _logger.LogInformation($"Start First API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

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
                      _httpClient.GetStreamAsync($"{secondapi}/secondapi");
                result = await
                  System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(jsonStream);
            }

            _logger.LogInformation($"Finish First API - get (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return result;
        }

        [HttpGet]
        [Route("queue")]
        public async Task<string> Queue()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();

            _logger.LogInformation($"Start First API - queue (RequestId - {requestId}): {DateTime.Now.ToString()}");

            string result = null;

            using (var _httpClient = new HttpClient())
            {
                result = await
                      _httpClient.GetStringAsync($"{secondapi}/secondapi/queue");
            }

            _logger.LogInformation($"Start First API - queue (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return result;
        }

        [HttpGet]
        [Route("httptrigger")]
        public async Task<string> HttpTrigger()
        {
            var requestId = this.HttpContext.Request.Headers["Request-Id"].ToString();

            _logger.LogInformation($"Start First API - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            string result = null;

            using (var _httpClient = new HttpClient())
            {
                result = await
                      _httpClient.GetStringAsync($"{secondapi}/secondapi/httptrigger");
            }

            _logger.LogInformation($"Start First API - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return result;
        }

        [HttpGet]
        [Route("signalr")]
        public async Task<string> SignalR()
        {
            var operationId = System.Diagnostics.Activity.Current.RootId;
            _logger.LogInformation($"signalr Controller OperationId => {operationId}");

            string result = null;

            using (var _httpClient = new HttpClient())
            {
                result = await
                      _httpClient.GetStringAsync($"{thirdapi}/thirdapi/signalr");
            }

            var queueInput = "{\"Test\":\"Sample\",\"OperationId\":\"" + operationId + "\"}";

            await GenerateAndSaveQueueMessage($"{connectionString}seyc-tracing-test-input", queueInput);

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
