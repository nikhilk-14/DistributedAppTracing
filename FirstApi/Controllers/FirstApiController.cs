using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirstApiController : ControllerBase
    {
        private readonly ILogger<FirstApiController> _logger;
        private readonly IConfiguration _config;

        public FirstApiController(ILogger<FirstApiController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [Route("get_data")]
        public async Task<string> GetData()
        {
            var operationId = System.Diagnostics.Activity.Current.RootId;
            _logger.LogInformation($"signalr Controller OperationId => {operationId}");

            string result = null;
            using (var _httpClient = new HttpClient())
            {
                result = await
                      _httpClient.GetStringAsync($"{_config.GetSection("Appsettings:Thirdapi").Value}/thirdapi/signalr");
            }

            var queueInput = "{\"Test\":\"Sample\",\"OperationId\":\"" + operationId + "\"}";

            await GenerateAndSaveQueueMessage($"{_config.GetSection("Appsettings:ServiceBusConnection").Value}seyc-tracing-test-input", queueInput);

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
