using Newtonsoft.Json;
using System;

namespace PortfolioTracker.MarketProvider.Models
{
    public class PriceQuote
    {
        [JsonProperty(PropertyName = "ticker")]
        public string TickerSymbol { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty(PropertyName = "last")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "open")]
        public decimal Open { get; set; }

        [JsonProperty(PropertyName = "prevClose")]
        public decimal Close { get; set; }
    }
}
    