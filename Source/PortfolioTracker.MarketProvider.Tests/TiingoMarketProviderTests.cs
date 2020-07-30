using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PortfolioTracker.MarketProvider.Models;
using PortfolioTracker.MarketProvider.Tiingo;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class IexMarketProviderTests
    {
        private string BaseUrl { get; } = "https://api.tiingo.com";
        private TiingoApiToken ApiToken { get; } = new TiingoApiToken("token");

        private MockHttpMessageHandler CreateMockPriceQuoteHandler(IEnumerable<PriceQuote> quotes)
        {
            var mockMessageHandler = new MockHttpMessageHandler();
            
            var url = $"{BaseUrl}/iex/?tickers={string.Join(",", quotes.Select(x => x.TickerSymbol))}&token={ApiToken}";
            var json = JsonConvert.SerializeObject(quotes);

            mockMessageHandler
            .When(HttpMethod.Get, "*")
            .Respond("application/json", json);

            return mockMessageHandler;
        }

        public static IEnumerable<object[]> GetPricesData()
        {
            var quotes = new List<PriceQuote>
            {
                new PriceQuote { TickerSymbol = "aapl", Price = 1.5M, Open = 1M, Close = 2M },
                new PriceQuote { TickerSymbol = "msft", Price = 3.5M, Open = 3M, Close = 4M },
                new PriceQuote { TickerSymbol = "lyft", Price = 5.5M, Open = 5M, Close = 6M },
                new PriceQuote { TickerSymbol = "fsly", Price = 7.5M, Open = 7M, Close = 8M }
            };

            for (int i = 1; i <= quotes.Count; i++)
            {
                yield return new object[] { quotes.Take(i) };
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetPricesData), DynamicDataSourceType.Method)]
        public async Task GetPricesAsync_ResultsReturnedInSameOrder(IEnumerable<PriceQuote> quotes)
        {
            // Arrange
            var mockMessageHandler = CreateMockPriceQuoteHandler(quotes.Reverse());
            var sut = new TiingoMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

            // Act
            var result = await sut.GetPricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

            // Assert
            CollectionAssert.AreEqual(quotes.Select(x => x.TickerSymbol).ToList(), result.Select(x => x.TickerSymbol).ToList());
        }

        [TestMethod]
        [DynamicData(nameof(GetPricesData), DynamicDataSourceType.Method)]
        public async Task GetPricesAsync_CorrectResultReturned(IEnumerable<PriceQuote> quotes)
        {
            // Arrange
            var mockMessageHandler = CreateMockPriceQuoteHandler(quotes);
            var sut = new TiingoMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

            // Act
            var result = await sut.GetPricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

            // Assert
            CollectionAssert.AreEqual(quotes.Select(x => x.Price).ToList(), result.Select(x => x.Price).ToList());
            CollectionAssert.AreEqual(quotes.Select(x => x.Open).ToList(), result.Select(x => x.Open).ToList());
            CollectionAssert.AreEqual(quotes.Select(x => x.Close).ToList(), result.Select(x => x.Close).ToList());
        }
    }
}
