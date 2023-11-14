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
    public static class SubmitJoke
    {

        [FunctionName("SubmitJoke")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JokeSubmit")] HttpRequest req,
            [ServiceBus("jokes-queue", Connection = "ServiceBusConnection")] ICollector<string> msg,
            ILogger log)
        {
            log.LogInformation("Add a joke");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Joke joke = JsonConvert.DeserializeObject<Joke>(requestBody)!;

            if (string.IsNullOrWhiteSpace(joke.author) || string.IsNullOrWhiteSpace(joke.question) || string.IsNullOrWhiteSpace(joke.answer))
            {
                return new OkObjectResult($"joke must have a question, answer, and author.");
            }

            if (string.IsNullOrWhiteSpace(joke.text))
            {
                joke.text = $"Q: {joke.question}  A: {joke.answer}";
            }

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
