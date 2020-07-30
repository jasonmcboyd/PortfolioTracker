using System;
using System.Collections.Generic;

namespace PortfolioTracker.MarketProvider.Models
{
    public class HistoricalPrices
    {
        public string TickerSymbol { get; set; }
        public DateTime StartDate { get; set; }
        public List<HistoricalPriceQuote> HistoricalPriceQuotes { get; set; }
    }
}
