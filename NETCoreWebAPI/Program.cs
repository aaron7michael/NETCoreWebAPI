using NETCoreWebAPI.Records;

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


app.MapGet("/weatherforecast", (int? numDays) =>
{
List<WeatherForecast> forecasts = new List<WeatherForecast>();
    foreach (int index in Enumerable.Range(1, numDays ?? 5))
    {
        forecasts.Add(new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 120)
        ));
    }
    return forecasts.ToArray();
})
.WithName("GetWeatherForecast");

app.Run();
