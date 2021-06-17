using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FirstFunction
{
    public class Function2
    {
        [FunctionName("Function2")]
        public async Task Run([ServiceBusTrigger("input-queue", Connection = "ServiceBusQueueConnection")] string myQueueItem, [DurableClient(TaskHub = "FunctionHub")] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation($"Function2 => OperationId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            log.LogInformation($"Function2 => Input => {myQueueItem}");

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function2_Orchestrator", input: myQueueItem);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        [FunctionName("Function2_Orchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Function2_Orchestrator => OperationId => {System.Diagnostics.Activity.Current.RootId}");
            var input = context.GetInput<string>();
            await context.CallActivityAsync("Function2_Activity", input);
            await context.CallActivityAsync("Function2_Activity_Output", input);
        }

        [FunctionName("Function2_Activity")]
        public async Task Function2_Activity([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation($"Function2_Activity => OperationId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            await Task.Run(() =>
            {
                Task.Delay(1000).Wait();
            });

            string secondapi = Environment.GetEnvironmentVariable("SecondApi");

            using (var _httpClient = new HttpClient())
            {
                await _httpClient.GetStringAsync($"{secondapi}/secondapi/get_data");
            }

            log.LogInformation($"Function2_Activity: {input}.");
        }

        [FunctionName("Function2_Activity_Output")]
        public async Task Function2_Activity_Output([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation($"Function2_Activity_Output => OperationId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");

            var connectionString = Environment.GetEnvironmentVariable("OutputServiceBusConnection");

            await Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                });
            await GenerateAndSaveQueueMessage($"{connectionString}", input);
            log.LogInformation($"Function2_Activity_Output => Pushed data in queue {input}.");
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
