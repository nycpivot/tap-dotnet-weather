using StackExchange.Redis;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Weather.Common
{
    public class ApiHelper : IApiHelper
    {
        public string WeatherApi { get; set; } = String.Empty;
        public string WeatherDbApi { get; set; } = String.Empty;
        public string WeatherBitUrl { get; set; } = String.Empty;
        public string WeatherBitKey { get; set; } = String.Empty;
        public string DefaultZipCode { get; set; } = String.Empty;

        public IWavefrontSender WavefrontSender { get; set;} = null;
        public IDatabase CacheDb { get; set; } = null;
    }
}
