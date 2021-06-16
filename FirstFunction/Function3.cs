using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FirstFunction
{
    public class Function3
    {
        public Function3()
        {

        }

        [FunctionName("Function3")]
        public async Task Run([ServiceBusTrigger("seyc-tracing-test-output", Connection = "ServiceBusQueueConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"Function3 => System.Diagnostics.Activity.Current.RootId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            log.LogInformation($"Function3 received queue message: {myQueueItem}");

            string fourthapi = "https://tracingfourthapi.azurewebsites.net";
            //string fourthapi = "https://localhost:5004";

            using (var _httpClient = new HttpClient())
            {
                await _httpClient.GetStringAsync($"{fourthapi}/fourthapi/signalr");
            }
        }
    }
}
