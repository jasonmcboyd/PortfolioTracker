using Newtonsoft.Json;
using PortfolioTracker.MarketProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace PortfolioTracker.MarketProvider.Tiingo
{
    public class TiingoMarketProvider : IMarketProvider
    {
        public TiingoMarketProvider(TiingoApiToken apiToken, HttpClient httpClient)
        {
            ApiToken = apiToken.Value;
            HttpClient = httpClient;
        }

        private string ApiToken { get; }
        private HttpClient HttpClient { get; }

        private string BaseUrl { get; } = "https://api.tiingo.com";

        public async Task<HistoricalPrices> GetHistoricalPriceQuotes(string tickerSymbol, DateTime startDate)
        {
            var url = $"{BaseUrl}/tiingo/daily/{tickerSymbol}/prices?startDate={startDate.ToString("yyyy-MM-dd")}&token={ApiToken}";

            var json = await HttpClient.GetStringAsync(url);
            
            return new HistoricalPrices
            {
                TickerSymbol = tickerSymbol,
                StartDate = startDate,
                HistoricalPriceQuotes = JsonConvert.DeserializeObject<List<HistoricalPriceQuote>>(json)
            };
        }

        public async Task<IList<PriceQuote>> GetPricesAsync(params string[] tickerSymbols)
        {
            if (tickerSymbols == null || tickerSymbols.Length == 0)
            {
                return new List<PriceQuote>();
            }

            var url = $"{BaseUrl}/iex/?tickers={string.Join(",", tickerSymbols)}&token={ApiToken}";

            var json = await HttpClient.GetStringAsync(url);

            var dict =
                JsonConvert.
                DeserializeObject<List<PriceQuote>>(json)
                .ToDictionary(x => x.TickerSymbol);

            return
                tickerSymbols
                .Select(x => dict[x])
                .ToList();
        }

        public IObservable<IList<PriceQuote>> GetPricesObservable(int seconds, params string[] tickerSymbols)
        {
            return
                Observable
                .Return(0L)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(seconds)))
                .SelectMany(_ => GetPricesAsync(tickerSymbols).ToObservable());
        }
    }
}
