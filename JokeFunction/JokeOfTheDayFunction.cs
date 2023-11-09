using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace JokeFunction
{
    public class JokeOftheDayFunction
    {
        private readonly JokeService _jokeService;
        private readonly IDistributedCache _cache;
        private const string CacheName = "JokeOfTheDay";

        public JokeOftheDayFunction(JokeService jokeService, IDistributedCache cache)
        {
            _jokeService = jokeService;
            _cache = cache;
        }

        [FunctionName("GetJokeOfTheDay")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JokeOfTheDay")] HttpRequest req,
            [CosmosDB(
                databaseName: "Jokes",
                containerName: "items",
                    Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetJokeOfTheDay.");

            // Check the cache first
            var cachedJokeString = await _cache.GetStringAsync(CacheName);
            if (!string.IsNullOrEmpty(cachedJokeString))
            {
                try
                {
                    // The deserialization doesn't have to be done, but we are doing it for fun.
                    var cachedJoke = JsonSerializer.Deserialize<Joke>(cachedJokeString);
                    return new OkObjectResult(cachedJoke);
                }
                catch // Catch specific exceptions
                {
                    // Just bail
                }
            }

            var joke = await _jokeService.GetRandomJoke(client, log);

            // Delete the joke from the cache
            //await _cache.RemoveAsync(CacheName);

            // Save the joke in the cache to expire in 24 hours
            await _cache.SetStringAsync(CacheName, JsonSerializer.Serialize(joke), new DistributedCacheEntryOptions
            {
                //AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            return new OkObjectResult(joke);

        }
    }
}
