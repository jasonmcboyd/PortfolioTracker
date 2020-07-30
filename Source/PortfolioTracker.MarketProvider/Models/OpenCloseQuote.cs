namespace PortfolioTracker.MarketProvider.Models
{
    public class OpenCloseQuote
    {
        public string TickerSymbol { get; set; }
        public decimal? Open { get; set; }
        public decimal? Close { get; set; }
        public bool QuoteFound { get; set; }
    }
}
