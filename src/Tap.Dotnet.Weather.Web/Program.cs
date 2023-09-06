using StackExchange.Redis;
using System.Collections.Immutable;
using Tap.Dotnet.Weather.Application;
using Tap.Dotnet.Weather.Application.Interfaces;
using Tap.Dotnet.Weather.Application.Models;
using Wavefront.SDK.CSharp.Common;
using Wavefront.SDK.CSharp.DirectIngestion;

var builder = WebApplication.CreateBuilder(args);

var serviceBindings = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT") ?? String.Empty;

// read environment variables
var weatherApi = Environment.GetEnvironmentVariable("WEATHER_API") ?? String.Empty;

// read secrets from files
var wavefrontUrl = "https://vmwareprod.wavefront.com"; // System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "host"));
var wavefrontToken = "bb074869-ed55-4a94-8607-b384cf8a39c1"; // System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "token"));
var cacheHost = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "host"));
var cachePort = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "port"));
var cachePassword = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "password"));

var cacheConfig = $"{cacheHost}:{cachePort},password={cachePassword}";

// setup Redis cache
var redisConnection = ConnectionMultiplexer.Connect(cacheConfig);
// var cacheServer = redisConnection.GetServer(cacheConnection);
var cacheDb = redisConnection.GetDatabase();

builder.Services.AddSingleton<IDatabase>(cacheDb);

// setup weather api service
builder.Services.AddSingleton<IWeatherApi>(new WeatherApi() { Url = weatherApi });

// setup wavefront service
var wfSender = new WavefrontDirectIngestionClient.Builder(wavefrontUrl, wavefrontToken).Build();
wfSender.SendSpan("Start", 0, 5000, "Program", Guid.NewGuid(), Guid.NewGuid(),
    ImmutableList.Create(Guid.NewGuid()), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "Program"), // "tap-dotnet-weather-web"),
                    new KeyValuePair<string, string>("service", "Start"),
                    new KeyValuePair<string, string>("http.method", "GET")), null);
//wfSender.Flush();

builder.Services.AddSingleton<WavefrontDirectIngestionClient>(wfSender);

// weather app
builder.Services.AddSingleton<IWeatherApplication, WeatherApplication>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
