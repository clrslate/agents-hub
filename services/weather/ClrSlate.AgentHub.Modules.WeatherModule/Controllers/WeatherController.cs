using Microsoft.AspNetCore.Mvc;
using ClrSlate.AgentHub.ApiService.Services;

namespace ClrSlate.AgentHub.ApiService.Controllers;

[ApiController, Route("weatherforecast")]
public class WeatherController(WeatherForecastService service) : ControllerBase
{
    [HttpGet]
    public IEnumerable<WeatherForecast> GetWeather() => service.GetWeather();
}