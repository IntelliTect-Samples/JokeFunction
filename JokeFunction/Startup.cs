using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(JokeFunction.Startup))]

namespace JokeFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<JokeService>();

        //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
    }
}

