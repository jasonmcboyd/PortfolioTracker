using System;

namespace PortfolioTracker.Wpf.Models
{
    public class HistoricalAction
    {
        public string TickerSymbol { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
    }
}
