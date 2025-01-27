namespace Greentube.Api.Dtos
{
    public record WeatherResponse
    (
     string City,
     string Description,
     string? Icon,
     int? TemperatureC,
     int? TemperatureF,
     DateTime? Sunrise,
     DateTime? Sunset,
     bool Success
    );
}