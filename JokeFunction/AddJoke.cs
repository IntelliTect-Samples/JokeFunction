using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace JokeFunction
{
    public class AddJoke
    {
        private JokeService _jokeService;

        public AddJoke(JokeService jokeService)
        {
            _jokeService = jokeService;
        }

        [FunctionName("AddJoke")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Joke")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("Add a joke to the database");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody)!;

            if (joke != null)
            {
                var result = true;
                var existingJoke = _jokeService.GetJoke(joke.id, client, log);
                if (existingJoke == null)
                {
                    // Add the joke if it doesn't exist
                    result = await _jokeService.AddJoke(joke, client, log);
                }

                if (result)
                {
                    result = await _jokeService.DeletePendingJoke(joke, client, log);
                    if (result) return new OkObjectResult("Joke Removed from Pending and added to active");
                }
            }
            return new BadRequestObjectResult("Joke not accepted");
        }
    }
}
