using Greentube.Api.Dtos;
using Greentube.Api.HttpClientWrapper;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Greentube.Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly WeatherApiSettings _weatherApiSettings;
        private readonly ILogger<WeatherService> _logger;
        public WeatherService(IHttpClientWrapper httpClientWrapper, 
                              IOptions<WeatherApiSettings> options,
                              ILogger<WeatherService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _weatherApiSettings = options.Value;
            _logger = logger;
        }
        public async Task<List<WeatherResponse>> GetWeatherData(string cities, string? unit)
        {            
            var weatherData = new List<WeatherResponse>();

            var cityList = cities.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(city => city.Trim())
                                 .Where(city => !string.IsNullOrWhiteSpace(city))
                                 .ToList();

            foreach (var city in cityList)
            {
                try
                {
                    var weatherResponse = await FetchWeatherDataAsync(city, unit);
                    weatherData.Add(weatherResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error fetching weather data for city: {city}");

                    weatherData.Add(new WeatherResponse
                    (
                        City: city,
                        Description: "An error occurred while fetching data.",
                        Icon: null,
                        TemperatureC: null,
                        TemperatureF: null,
                        Sunrise: null,
                        Sunset: null,
                        Success: false
                    ));
                }
            }

            return weatherData;
        }

        private async Task<WeatherResponse> FetchWeatherDataAsync(string city, string? unit)
        {
            var response = await _httpClientWrapper.GetAsync($"{_weatherApiSettings.BaseUrl}?q={city}&appid={_weatherApiSettings.ApiKey}&units={unit}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching data for city: {city}, Status code: {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<JsonElement>(jsonString);

            double temperature = responseData.GetProperty("main").GetProperty("temp").GetDouble();
            int temperatureC, temperatureF;

            if (unit == "metric")
            {
                temperatureC = (int)Math.Round(temperature);
                temperatureF = (int)Math.Round(temperature * 9 / 5 + 32);
            }
            else if (unit == "imperial")
            {
                temperatureF = (int)Math.Round(temperature);
                temperatureC = (int)Math.Round((temperature - 32) * 5 / 9);
            }
            else
            {
                throw new ArgumentException("Invalid unit. Must be 'metric' or 'imperial'.");
            }

            return new WeatherResponse
            (
                City : city,
                Description: responseData.GetProperty("weather")[0].GetProperty("description").GetString(),
                Icon: $"https://openweathermap.org/img/wn/{responseData.GetProperty("weather")[0].GetProperty("icon").GetString()}@2x.png",
                TemperatureC: temperatureC,
                TemperatureF: temperatureF,
                Sunrise: DateTimeOffset.FromUnixTimeSeconds(responseData.GetProperty("sys").GetProperty("sunrise").GetInt64()).DateTime,
                Sunset : DateTimeOffset.FromUnixTimeSeconds(responseData.GetProperty("sys").GetProperty("sunset").GetInt64()).DateTime,
                Success : true
            );
        }
    }
}
