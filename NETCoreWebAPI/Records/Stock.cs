namespace NETCoreWebAPI.Records
{
    using Newtonsoft.Json;
    using System;
    using System.Text.Json.Serialization;

    internal record Stock(string Symbol)
    {
        [JsonPropertyName("Yesterday's Price")]
        public decimal PrevPrice { get; set; }
        [JsonPropertyName("Today's Price")]
        public decimal CurrPrice { get; set; }
        [JsonPropertyName("Percent Change")]
        public string PercentChange => GetPercentChange(PrevPrice, CurrPrice);

        [JsonPropertyName("Recommended Action")]
        public string RecAction => GetRecommendedAction(PrevPrice, CurrPrice);
        
        public Stock(string Symbol, decimal PrevPrice, decimal CurrPrice) : this(Symbol)
        {
            this.PrevPrice = PrevPrice;
            this.CurrPrice = CurrPrice;
        }

        private string GetRecommendedAction(decimal prevPrice, decimal currPrice)
        {
            return prevPrice < currPrice ? "SELL" : "BUY";   
        }
        private string GetPercentChange(decimal prevPrice, decimal currPrice)
        {
            decimal percentChange = (currPrice - prevPrice) / prevPrice * 100;

            return $"%{Math.Round(percentChange, 2)}";
        }
    }

    internal record StockDTO(string Symbol)
    {
        public decimal PrevPrice { get; set; }
        public decimal CurrPrice { get; set; }
    }
}
