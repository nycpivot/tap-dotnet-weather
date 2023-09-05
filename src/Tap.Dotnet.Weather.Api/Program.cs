using Tap.Dotnet.Common.Interfaces;
using Tap.Dotnet.Common;
using Wavefront.SDK.CSharp.DirectIngestion;

var builder = WebApplication.CreateBuilder(args);

var serviceBindings = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT") ?? String.Empty;

var weatherDbApi = Environment.GetEnvironmentVariable("WEATHER_DB_API") ?? String.Empty;

var weatherBitUrl = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "weather-bit-resource-claim", "host"));
var weatherBitKey = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "weather-bit-resource-claim", "key"));
var wavefrontUrl = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "host"));
var wavefrontToken = System.IO.File.ReadAllText(Path.Combine(serviceBindings, "wavefront-api-resource-claim", "token"));

var wfSender = new WavefrontDirectIngestionClient.Builder(wavefrontUrl, wavefrontToken).Build();

var apiHelper = new ApiHelper()
{
    WeatherBitUrl = weatherBitUrl,
    WeatherBitKey = weatherBitKey,
    WeatherDbApi = weatherDbApi,
    WavefrontSender = wfSender
};

builder.Services.AddSingleton<IApiHelper>(apiHelper);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
