using JokeFunction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder();
builder.ConfigureFunctionsWorkerDefaults();
//builder.ConfigureServices(services =>
//{
//    services.AddScoped<JokeService>();
//});

var host = builder.Build();
host.Run();

