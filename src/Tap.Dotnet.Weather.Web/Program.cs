using StackExchange.Redis;
using Tap.Dotnet.Weather.Application;
using Tap.Dotnet.Weather.Application.Interfaces;
using Tap.Dotnet.Weather.Common;
using Wavefront.SDK.CSharp.DirectIngestion;

var builder = WebApplication.CreateBuilder(args);

var serviceBindings = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT") ?? String.Empty;

// read environment variables
var weatherApi = Environment.GetEnvironmentVariable("WEATHER_API") ?? String.Empty;

// read secrets from files
var wavefrontUrl = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "host"));
var wavefrontToken = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "token"));
var cacheConnection = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "redis-cache-class-claim", "connection"));

var wfSender = new WavefrontDirectIngestionClient.Builder(wavefrontUrl, wavefrontToken).Build();

var redisConnection = ConnectionMultiplexer.Connect(cacheConnection);
//var cacheServer = redisConnection.GetServer(cacheConnection);
var cacheDb = redisConnection.GetDatabase();

builder.Services.AddSingleton<IDatabase>(cacheDb);

var apiHelper = new ApiHelper()
{
    WeatherApi = weatherApi,
    WavefrontSender = wfSender,
    //CacheDb = cacheDb
};

builder.Services.AddSingleton<IApiHelper>(apiHelper);
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
