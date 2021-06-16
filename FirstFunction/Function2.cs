using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FirstFunction
{
    public class Function2
    {
        public Function2()
        {

        }


        [FunctionName("Function2")]
        public async Task Run([ServiceBusTrigger("seyc-tracing-test-input", Connection = "ServiceBusQueueConnection")] string myQueueItem, [DurableClient(TaskHub = "Abc")] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation($"Function2 => System.Diagnostics.Activity.Current.RootId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            log.LogInformation($"Function2 => Input => {myQueueItem}");

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function2_Orchestrator", input: myQueueItem);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        [FunctionName("Function2_Orchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Function2_Orchestrator => System.Diagnostics.Activity.Current.RootId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            var input = context.GetInput<string>();
            await context.CallActivityAsync("Function2_Activity", input);
            await context.CallActivityAsync("Function2_Activity_Output", input);
        }

        [FunctionName("Function2_Activity")]
        public async Task Function2_Activity([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation($"Function2_Activity => System.Diagnostics.Activity.Current.RootId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            await Task.Run(() =>
            {
                Task.Delay(1000).Wait();
            });

            string secondapi = "https://tracingsecondapi.azurewebsites.net";
            //string secondapi = "https://localhost:5002";

            using (var _httpClient = new HttpClient())
            {
                await _httpClient.GetStringAsync($"{secondapi}/secondapi/signalr");
            }

            log.LogInformation($"Function2_Activity: {input}.");
        }

        [FunctionName("Function2_Activity_Output")]
        //[return: ServiceBus("seyc-tracing-test-output", Connection = "ServiceBusQueueConnection")]
        public async Task Function2_Activity_Output([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation($"Function2_Activity_Output => System.Diagnostics.Activity.Current.RootId => {System.Diagnostics.Activity.Current.RootId} => {DateTime.Now}");
            var connectionString = "Endpoint=sb://seyc-cms-api-rules-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Ot3SDFOTG2NQMkD9v12DV1gn3Bto9kukgn+6YCs9Xi0=;EntityPath=";
            await Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                });
            await GenerateAndSaveQueueMessage($"{connectionString}seyc-tracing-test-output", input);
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
