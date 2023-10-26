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
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ToDoItems",
                containerName: "Items",
                Connection = "CosmosDBConnection")]out dynamic document,
            ILogger log)
        {
            log.LogInformation("Add a joke to the database");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody);

            if (joke != null)
            {
                joke.id = Guid.NewGuid();
                document = joke;

                log.LogInformation($"http triggered to add joke: {joke}");

                return new OkObjectResult($"add joke");
            }
            else
            {
                log.LogInformation("no joke!");
                return new BadRequestObjectResult("Need a joke");
            }
        }
    }
}
