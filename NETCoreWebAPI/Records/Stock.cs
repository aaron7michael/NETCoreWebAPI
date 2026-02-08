namespace NETCoreWebAPI.Records
{
    using Newtonsoft.Json;
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.Json.Serialization;

    internal record Stock(string Symbol)
    {
        [JsonPropertyName("Yesterday's Price")]
        public int prevPrice { get; set; }
        [JsonPropertyName("Today's Price")]
        public int currPrice { get; set; }
        [JsonPropertyName("Percent Change")]
        public string percentChange => GetPercentChange(prevPrice, currPrice);

        [JsonPropertyName("Recommended Action")]
        public string recAction => GetRecommendedAction(prevPrice, currPrice);
        

        private string GetRecommendedAction(int prevPrice, int currPrice)
        {
            return prevPrice < currPrice ? "SELL" : "BUY";   
        }
        private string GetPercentChange(int prevPrice, int currPrice)
        {
            throw new NotImplementedException();
        }
    }
}
