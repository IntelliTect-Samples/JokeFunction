using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JokeFunction
{
    public static class AddJoke
    {

        [FunctionName("AddJoke")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("outqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> msg,
            ILogger log)
        {
            log.LogInformation("Add a joke");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody);

            if (joke != null)
            {
                msg.Add(JsonConvert.SerializeObject(joke));
                return new OkObjectResult($"joke added successfully");
            }
            else
            {
                return new BadRequestObjectResult("Need a joke");
            }
        }
    }
}
