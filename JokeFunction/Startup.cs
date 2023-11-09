using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(JokeFunction.Startup))]

namespace JokeFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<JokeService>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.GetContext().Configuration.GetValue<string>("RedisCacheConnectionString");
            options.InstanceName = "Joke";
        });
    }
}

