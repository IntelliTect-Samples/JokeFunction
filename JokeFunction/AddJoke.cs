using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JokeFunction
{
    public static class AddJoke
    {

        [FunctionName("AddJoke")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Joke")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                Connection = "CosmosDBConnection")]out dynamic document,
            ILogger log)
        {
            log.LogInformation("Add a joke to the database");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody);

            if (joke != null)
            {
                joke.id = Guid.NewGuid().ToString();
                document = joke;

                log.LogInformation($"http triggered to add joke: {joke}");

                return new OkObjectResult($"add joke");
            }
            else
            {
                document = null;
                log.LogInformation("no joke!");
                return new BadRequestObjectResult("Need a joke");
            }
        }
    }
}
