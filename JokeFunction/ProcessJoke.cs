using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace JokeFunction
{
    public class ProcessJoke
    {
        [FunctionName("ProcessJoke")]
        public void Run([ServiceBusTrigger("jokes-queue", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
