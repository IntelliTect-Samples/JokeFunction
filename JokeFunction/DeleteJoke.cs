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
    public class DeleteJoke
    {
        private JokeService _jokeService;

        public DeleteJoke(JokeService jokeService)
        {
            _jokeService = jokeService;
        }

        [FunctionName("DeleteJoke")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Joke")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("Delete joke");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody)!;

            if (joke != null)
            {
                var result = await _jokeService.DeletePendingJoke(joke, client, log);
                if (result) return new OkObjectResult("Joke Deleted from Pending");
            }
            return new BadRequestObjectResult("Joke not Deleted");

        }
    }
}
