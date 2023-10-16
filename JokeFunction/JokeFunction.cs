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
    public static class JokeFunction
    {
        [FunctionName("Joke")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string? search = req.Query["search"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            search = search ?? data?.search;

            var js = new JokeService(log);
            var joke = js.GetRandomJoke(search);
            if (joke == null)
            {
                return new OkObjectResult($"No jokes found with '{search}'");
            }
            return new OkObjectResult(joke);
            
        }
    }
}