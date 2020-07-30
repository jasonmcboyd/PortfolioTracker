using PortfolioTracker.MarketProvider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfolioTracker.MarketProvider
{
    public interface IMarketProvider
    {
        Task<IList<PriceQuote>> GetPricesAsync(params string[] tickerSymbols);
        IObservable<IList<PriceQuote>> GetPricesObservable(int seconds, params string[] tickerSymbols);
        Task<HistoricalPrices> GetHistoricalPriceQuotes(string tickerSymbol, DateTime startDate);
    }
}
