namespace NETCoreWebAPI.Records
{
    internal record WeatherForecast(DateOnly Date, int TemperatureF)
    {
        public int TemperatureC => (int)((TemperatureF - 32) * 0.5556);
        public string Summary => GetSummary(TemperatureF);

        private string GetSummary(int tempF)
        {
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
            string summary = summaries[summaries.Keys.First()];
            foreach (int t in summaries.Keys)
            {
                summary = tempF >= t ? summaries[t] : summary;
            }
            return summary;
        }
    }
}