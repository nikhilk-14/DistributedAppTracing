using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FirstFunction
{
    public class Function1
    {
        public Function1()
        {

        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var requestId = req.Headers["Request-Id"].ToString();

            log.LogInformation($"Start Function1 - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            log.LogInformation($"C# HTTP trigger function processed a request. - {requestId}");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            log.LogInformation($"Finish Function1 - httptrigger (RequestId - {requestId}): {DateTime.Now.ToString()}");

            return new OkObjectResult(responseMessage);
        }
    }
}

