using Volo.Abp.DependencyInjection;

namespace ClrSlate.AgentHub.ApiService.Services;

public class WeatherForecastService: ITransientDependency
{
    private static readonly string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
    public IEnumerable<WeatherForecast> GetWeather() 
        => Enumerable.Range(1, 5)
            .Select(index =>new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ));
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
