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
    public class PendingJokes
    {
        private JokeService _jokeService;

        public PendingJokes(JokeService jokeService)
        {
            _jokeService = jokeService;
        }
        
        [FunctionName("PendingJokes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PendingJokes")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "pendingItems",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var joke = await _jokeService.GetPendingJokes(client, log);

            if (joke == null)
            {
                return new OkObjectResult($"No jokes found with Joke Id {req.Query["id"]} and author {req.Query["partitionKey"]}");
            }
            return new OkObjectResult(joke);
        }
    }
}
