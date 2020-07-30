using Newtonsoft.Json;
using System;

namespace PortfolioTracker.MarketProvider.Models
{
    public class HistoricalPriceQuote
    {
        [JsonProperty(PropertyName = "date")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty(PropertyName = "open")]
        public decimal Open { get; set; }

        [JsonProperty(PropertyName = "high")]
        public decimal High { get; set; }

        [JsonProperty(PropertyName = "low")]
        public decimal Low { get; set; }

        [JsonProperty(PropertyName = "close")]
        public decimal Close { get; set; }
    }
}
