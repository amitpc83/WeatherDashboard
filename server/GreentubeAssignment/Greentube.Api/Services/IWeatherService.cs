using Greentube.Api.Dtos;

namespace Greentube.Api.Services
{
    public interface IWeatherService
    {
        Task<List<WeatherResponse>> GetWeatherData(string cities, string? unit);
    }
}
