using Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JokeFunction
{
    public class JokeService
    {

        private static Joke[]? _jokeList = null;
        private ILogger _logger;

        public JokeService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<Joke> GetRandomJoke(CosmosClient client, ILogger log)
        {
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

            return joke;

        }

        public async Task<IEnumerable<Joke>> GetPendingJokes(CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("Jokes").GetContainer("PendingItems");

            log.LogInformation($"Getting all pending jokes");

            QueryDefinition queryDefinition = new QueryDefinition(
                "SELECT * FROM PendingItems i");

            List<Joke> jokes = new();
            using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    jokes.AddRange((await resultSet.ReadNextAsync()));
                }
            }

            return jokes;
        }


        public async Task<bool> DeletePendingJoke(Joke joke, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("Jokes").GetContainer("PendingItems");

            log.LogInformation($"Delete Pending Joke {joke.id} in {joke.author}");

            var result = await container.DeleteItemAsync<Joke>(joke.id, new PartitionKey(joke.author));

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                log.LogInformation($"Deleted Pending Joke {joke.id} in {joke.author}");

                return true;
            }

            log.LogInformation($"Failed to Delete Pending Joke {joke.id} in {joke.author}");
            return false;
        }

        public async Task<bool> AddJoke(Joke joke, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("Jokes").GetContainer("items");

            log.LogInformation($"Add Joke {joke.id} in {joke.author}");

            var result = await container.CreateItemAsync<Joke>(joke);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                log.LogInformation($"Added Joke {joke.id} in {joke.author}");

                return true;
            }

            log.LogInformation($"Failed to Add Joke {joke.id} in {joke.author}");
            return false;
        }

        public async Task<Joke> GetJoke(string id, CosmosClient client, ILogger log)
        {
            Container container = client.GetDatabase("Jokes").GetContainer("items");
            
            QueryDefinition queryDefinitionJoke = new QueryDefinition(
                @$"SELECT * FROM items i WHERE i.id=""{id}""");

            Joke? joke = null;
            using (FeedIterator<Joke> resultSet = container.GetItemQueryIterator<Joke>(queryDefinitionJoke))
            {
                joke = (await resultSet.ReadNextAsync()).First();
            }

            return joke;
        }
    }
}
