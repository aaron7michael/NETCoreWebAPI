var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new Dictionary<int, string>()
        {
            {  20, "Freezing"   },
            {  30, "Bracing"    },
            {  40, "Chilly"     },
            {  50, "Cool"       },
            {  60, "Mild"       },
            {  70, "Warm"       },
            {  80, "Balmy"      },
            {  90, "Hot"        },
            { 100, "Sweltering" },
            { 110, "Scorching"  }
        };

app.MapGet("/weatherforecast", (int? numDays) =>
{
List<WeatherForecast> forecasts = new List<WeatherForecast>();
    foreach (int index in Enumerable.Range(1, numDays ?? 5))
    {
        int temp = Random.Shared.Next(-20, 120);
        string summary = summaries[summaries.Keys.First()];
        foreach (int t in summaries.Keys)
        {
            summary = temp >= t ? summaries[t] : summary;
        }
        forecasts.Add(new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            temp,
            summary
        ));
    }
    return forecasts.ToArray();
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureF, string? Summary)
{
    public int TemperatureC = (int)(TemperatureF * 0.5556) - 32;
}
