﻿using Newtonsoft.Json;
using System.Net;
using Tap.Dotnet.Weather.Application.Interfaces;
using Tap.Dotnet.Weather.Application.Models;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Weather.Application
{
    public class WeatherApplication : IWeatherApplication
    {
        private readonly IWeatherApi weatherApi;
        private readonly IWavefrontSender wavefrontSender;
        // private readonly IApiHelper apiHelper;

        public WeatherApplication(IWeatherApi weatherApi, IWavefrontSender sender)
        {
            this.weatherApi = weatherApi;
            this.wavefrontSender = sender;
        }

        public HomeViewModel GetForecast(string zipCode)
        {
            var homeViewModel = GetHomeView(zipCode);

            return homeViewModel;
        }

        public HomeViewModel SaveFavorite(string zipCode)
        {
            var homeViewModel = new HomeViewModel();

            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //this.apiHelper.WavefrontSender.SendSpan(
            //    "Get", 0, 1, "ForecastController", traceId, spanId,
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-web-mvc"),
            //        new KeyValuePair<string, string>("service", "GetWeather"),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherApi.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());

                    var result = httpClient.GetAsync($"favorites/{zipCode}").Result;
                }
            }

            homeViewModel = GetHomeView(zipCode);

            return homeViewModel;
        }

        private HomeViewModel GetHomeView(string zipCode)
        {
            var homeViewModel = new HomeViewModel();

            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            //this.apiHelper.WavefrontSender.SendSpan(
            //    "Get", 0, 1, "ForecastController", traceId, spanId,
            //    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
            //    ImmutableList.Create(
            //        new KeyValuePair<string, string>("application", "tap-dotnet-web-mvc"),
            //        new KeyValuePair<string, string>("service", "GetWeather"),
            //        new KeyValuePair<string, string>("http.method", "GET")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherApi.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());

                    // get saved favorite zip codes
                    var favoritesResponse = httpClient.GetAsync("favorites").Result;
                    if (favoritesResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = favoritesResponse.Content.ReadAsStringAsync().Result;
                        homeViewModel.Favorites = JsonConvert.DeserializeObject<IList<FavoriteViewModel>>(content);
                    }

                    // get weather forecast for incoming zip code
                    var forecastResponse = httpClient.GetAsync($"forecast/{zipCode}").Result;
                    if (forecastResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var content = forecastResponse.Content.ReadAsStringAsync().Result;

                        homeViewModel.WeatherInfo = JsonConvert.DeserializeObject<WeatherInfoViewModel>(content);
                        homeViewModel.WeatherInfo.ZipCode = zipCode;
                    }
                }
            }

            return homeViewModel;
        }
    }
}