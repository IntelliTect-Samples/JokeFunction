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
    public static class JokeFunction
    {
        [FunctionName("GetRandomJoke")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Joke")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            //[CosmosDB(
            //    databaseName: "Jokes",
            //    containerName: "items",
            //    Connection = "CosmosDBConnection",
            //    Id = "{Query.id}",
            //    PartitionKey = "{Query.partitionKey}")]Joke joke,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // string? search = req.Query["search"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // search = search ?? data?.search;

            // var js = new JokeService(log);
            // var joke = js.GetRandomJoke(search);
            Container container = client.GetDatabase("Jokes").GetContainer("items");

            log.LogInformation($"Searching for Joke Count");

            QueryDefinition queryDefinition = new QueryDefinition(
                "SELECT value Count(i) FROM items i");

            int count = 0;
            using (FeedIterator<int> resultSet = container.GetItemQueryIterator<int>(queryDefinition))
            {
                count = (await resultSet.ReadNextAsync()).First();
            }
            log.LogInformation($"{count} jokes found");

            // Random number between 0 and count
            var rnd = new Random();
            int offset = rnd.Next(count);

            log.LogInformation($"Grabbing joke {offset} of {count}");


            QueryDefinition queryDefinitionJoke = new QueryDefinition(
                $"SELECT * FROM items i OFFSET {offset} LIMIT 1");

            Joke? joke = null;
            using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinitionJoke))
            {
                joke = (await resultSet.ReadNextAsync()).First();
            }


            if (joke == null)
            {
                return new OkObjectResult($"No jokes found with Joke Id {req.Query["id"]} and author {req.Query["partitionKey"]}");
            }
            return new OkObjectResult(joke);

        }
    }
}
