using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Linq;

namespace JokeFunction
{
    public static class DeleteJoke
    {
        [FunctionName("DeleteJoke")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Joke")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("Delete joke");

            string? id = req.Query["id"];
            string? partitionKey = req.Query["partitionKey"];

            Container container = client.GetDatabase("Jokes").GetContainer("items");
            ItemResponse<Joke> jokeResponse = await container.DeleteItemAsync<Joke>(id, new PartitionKey(partitionKey));
            log.LogInformation($"Deleted joke {partitionKey},{id}");
            return new OkObjectResult($"Deleted joke {id}");
        }
    }
}
