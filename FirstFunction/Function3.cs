using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FirstFunction
{
    public class Function3
    {
        [FunctionName("Function3")]
        public async Task Run([ServiceBusTrigger("output-queue", Connection = "ServiceBusQueueConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"Function3 => OperationId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            log.LogInformation($"Function3 received queue message: {myQueueItem}");

            string fourthapi = Environment.GetEnvironmentVariable("FourthApi");

            using (var _httpClient = new HttpClient())
            {
                await _httpClient.GetStringAsync($"{fourthapi}/fourthapi/get_data");
            }
        }
    }
}
