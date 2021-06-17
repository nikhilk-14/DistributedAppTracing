using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FirstFunction
{
    public class Function4
    {
        [FunctionName("Function4")]
        public async Task Run([ServiceBusTrigger("test-queue", Connection = "ServiceBusQueueConnection")]string myQueueItem, ILogger log)
        {
            await Task.Run(() =>
            {
                Task.Delay(1000).Wait();
            });

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
