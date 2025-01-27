using Greentube.Api.Dtos;
using Greentube.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Greentube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private const string sessionKeyUnit = "TemperatureUnit";
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        [Route("WeatherData")]
        public async Task<IActionResult> WeatherDataAsync([FromQuery] string cities)
        {
            if (string.IsNullOrEmpty(cities))
            {
                return BadRequest("cities is required parameter");
            }
            var unit = HttpContext.Session.GetString(sessionKeyUnit) ?? "metric";            
            var weatherData = await _weatherService.GetWeatherData(cities, unit);
            return Ok(weatherData);
        }

        [HttpGet]
        [Route("TemperatureUnit")]
        public ActionResult<string> GetTemperatureUnit()
        {
            var unit = HttpContext.Session.GetString(sessionKeyUnit) ?? "metric";

            return Ok(new {unit});
        }

        // POST api/unit
        [HttpPost]
        [Route("TemperatureUnit")]
        public IActionResult SetTemperatureUnit([FromBody] TemperatureUnit temperatureUnit)
        {
            if (temperatureUnit == null || string.IsNullOrEmpty(temperatureUnit.Unit))
            {
                return BadRequest("Invalid unit value.");
            }
            if (temperatureUnit.Unit != "metric" && temperatureUnit.Unit != "imperial")
            {
                return BadRequest("Unit must be either 'metric' or 'imperial'.");
            }

            HttpContext.Session.SetString(sessionKeyUnit, temperatureUnit.Unit);
            return Ok();

        }

    }
}
