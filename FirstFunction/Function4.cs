using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FirstFunction
{
    public class Function4
    {
        public Function4()
        {

        }

        [FunctionName("Function4")]
        public async Task Run([ServiceBusTrigger("seyc-tracing-test", Connection = "ServiceBusQueueConnection")]string myQueueItem, ILogger log)
        {
            await Task.Run(() =>
            {
                Task.Delay(1000).Wait();
            });

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
