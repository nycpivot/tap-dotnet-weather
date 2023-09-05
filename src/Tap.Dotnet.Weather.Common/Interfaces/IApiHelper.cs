using StackExchange.Redis;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Common.Interfaces
{
    public interface IApiHelper
    {
        string WeatherApi { get; set; }
        string WeatherBitUrl { get; set; }
        string WeatherBitKey { get; set; }
        string DefaultZipCode { get; set; }
        string WeatherDbApi { get; set; }
        IWavefrontSender WavefrontSender { get; set; }
        IDatabase CacheDb { get; set; }
    }
}
