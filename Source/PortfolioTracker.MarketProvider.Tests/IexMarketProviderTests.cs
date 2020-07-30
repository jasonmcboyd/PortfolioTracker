//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PortfolioTracker.MarketProvider.Iex;
//using RichardSzalay.MockHttp;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace UnitTestProject1
//{
//    [TestClass]
//    public class IexMarketProviderTests
//    {
//        private string BaseUrl { get; } = "https://cloud.iexapis.com/v1";
//        private ApiToken ApiToken { get; } = new ApiToken("token");

//        private MockHttpMessageHandler CreateMockGetPricesHandler(IEnumerable<(string TickerSymbol, decimal Price)> quotes)
//        {
//            var mockMessageHandler = new MockHttpMessageHandler();

//            foreach (var quote in quotes)
//            {
//                var url = $"{BaseUrl}/stock/{quote.TickerSymbol}/quote/latestPrice?token={ApiToken.Value}";

//                mockMessageHandler
//                .When(HttpMethod.Get, url)
//                .Respond("application/json", quote.Price.ToString());
//            }

//            return mockMessageHandler;
//        }

//        private MockHttpMessageHandler CreateMockGetOpenCloseQuoteHandler(IEnumerable<(string TickerSymbol, decimal? Open, decimal? Close)> quotes)
//        {
//            var mockMessageHandler = new MockHttpMessageHandler();

//            foreach (var quote in quotes)
//            {
//                var url = $"{BaseUrl}/stock/{quote.TickerSymbol}/quote/open?token={ApiToken.Value}";

//                if (quote.Open != null)
//                {
//                    mockMessageHandler
//                    .When(HttpMethod.Get, url)
//                    .Respond("application/json", quote.Open.Value.ToString());
//                }
//                else
//                {
//                    mockMessageHandler
//                    .When(HttpMethod.Get, url)
//                    .Respond(HttpStatusCode.NotFound);
//                }

//                url = $"{BaseUrl}/stock/{quote.TickerSymbol}/quote/close?token={ApiToken.Value}";

//                if (quote.Open != null)
//                {
//                    mockMessageHandler
//                    .When(HttpMethod.Get, url)
//                    .Respond("application/json", quote.Close.Value.ToString());
//                }
//                else
//                {
//                    mockMessageHandler
//                    .When(HttpMethod.Get, url)
//                    .Respond(HttpStatusCode.NotFound);
//                }
//            }

//            return mockMessageHandler;
//        }

//        public static IEnumerable<object[]> GetPricesData()
//        {
//            var quotes = new List<(string TickerSymbol, decimal price)>
//            {
//                ("aapl", 1M),
//                ("msft", 2M),
//                ("lyft", 3M)
//            };

//            for (int i = 1; i <= quotes.Count; i++)
//            {
//                yield return new object[] { quotes.Take(i) };
//            }
//        }

//        public static IEnumerable<object[]> GetOpenCloseData()
//        {
//            var quotes = new List<(string TickerSymbol, decimal? Open, decimal? Close)>
//            {
//                ("aapl", 1M, 2M),
//                ("msft", 3M, 4M),
//                ("lyft", 5M, 6M),
//                ("swppx", null, null)
//            };

//            for (int i = 1; i <= quotes.Count; i++)
//            {
//                yield return new object[] { quotes.Take(i) };
//            }
//        }

//        [TestMethod]
//        [DynamicData(nameof(GetPricesData), DynamicDataSourceType.Method)]
//        public async Task GetPricesAsync_ResultsReturnedInSameOrder(IEnumerable<(string TickerSymbol, decimal Price)> quotes)
//        {
//            // Arrange
//            var mockMessageHandler = CreateMockGetPricesHandler(quotes);
//            var sut = new IexMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

//            // Act
//            var result = await sut.GetPricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

//            // Assert
//            CollectionAssert.AreEqual(quotes.Select(x => x.TickerSymbol).ToList(), result.Select(x => x.TickerSymbol).ToList());
//        }

//        [TestMethod]
//        [DynamicData(nameof(GetPricesData), DynamicDataSourceType.Method)]
//        public async Task GetPricesAsync_CorrectPriceReturned(IEnumerable<(string TickerSymbol, decimal Price)> quotes)
//        {
//            // Arrange
//            var mockMessageHandler = CreateMockGetPricesHandler(quotes);
//            var sut = new IexMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

//            // Act
//            var result = await sut.GetPricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

//            // Assert
//            CollectionAssert.AreEqual(quotes.Select(x => x.Price).ToList(), result.Select(x => x.Price).ToList());
//        }

//        [TestMethod]
//        [DynamicData(nameof(GetOpenCloseData), DynamicDataSourceType.Method)]
//        public async Task GetOpenClosePricesAsync_ResultsReturnedInSameOrder(IEnumerable<(string TickerSymbol, decimal? Open, decimal? Close)> quotes)
//        {
//            // Arrange
//            var mockMessageHandler = CreateMockGetOpenCloseQuoteHandler(quotes);
//            var sut = new IexMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

//            // Act
//            var result = await sut.GetOpenClosePricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

//            // Assert
//            CollectionAssert.AreEqual(quotes.Select(x => x.TickerSymbol).ToList(), result.Select(x => x.TickerSymbol).ToList());
//        }

//        [TestMethod]
//        [DynamicData(nameof(GetOpenCloseData), DynamicDataSourceType.Method)]
//        public async Task GetOpenClosePricesAsync_CorrectResultReturned(IEnumerable<(string TickerSymbol, decimal? Open, decimal? Close)> quotes)
//        {
//            // Arrange
//            var mockMessageHandler = CreateMockGetOpenCloseQuoteHandler(quotes);
//            var sut = new IexMarketProvider(ApiToken, new HttpClient(mockMessageHandler));

//            // Act
//            var result = await sut.GetOpenClosePricesAsync(quotes.Select(x => x.TickerSymbol).ToArray());

//            // Assert
//            CollectionAssert.AreEqual(quotes.Select(x => x.Open).ToList(), result.Select(x => x.Open).ToList());
//            CollectionAssert.AreEqual(quotes.Select(x => x.Close).ToList(), result.Select(x => x.Close).ToList());
//            CollectionAssert.AreEqual(quotes.Select(x => x.Close.HasValue).ToList(), result.Select(x => x.QuoteFound).ToList());
//        }
//    }
//}
