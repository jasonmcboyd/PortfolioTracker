using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioTracker.EF;
using PortfolioTracker.MarketProvider.Models;
using PortfolioTracker.Wpf.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Wpf.Tests.DataStructures
{
    [TestClass]
    public class PositionValueTimeSeriesTests
    {
        public static IEnumerable<object[]> GetData()
        {
            var transactions =
                Enumerable
                .Range(1, 6)
                .Select(x => new Transaction
                {
                    Quantity = 1,
                    TickerSymbol = x % 2 == 0 ? "msft" : "lyft",
                    TransactionDate = new DateTime(2020, 1, x),
                    TransactionType = "b"
                })
                .ToList();
            transactions.Last().TransactionType = "s";

            var quotes =
                Enumerable
                .Range(1, 6)
                .Select(x => new HistoricalPriceQuote
                {
                    Timestamp = new DateTime(2020, 1, x),
                    Close = x
                })
                .ToList();

            var lyftExpected = new List<DateValue>
            {
                new DateValue(new DateTime(2020, 1, 1), 1M),
                new DateValue(new DateTime(2020, 1, 2), 2M),
                new DateValue(new DateTime(2020, 1, 3), 6M),
                new DateValue(new DateTime(2020, 1, 4), 8M),
                new DateValue(new DateTime(2020, 1, 5), 15M),
                new DateValue(new DateTime(2020, 1, 6), 18M)
            };

            var msftExpected = new List<DateValue>
            {
                new DateValue(new DateTime(2020, 1, 1), 0M),
                new DateValue(new DateTime(2020, 1, 2), 2M),
                new DateValue(new DateTime(2020, 1, 3), 3M),
                new DateValue(new DateTime(2020, 1, 4), 8M),
                new DateValue(new DateTime(2020, 1, 5), 10M),
                new DateValue(new DateTime(2020, 1, 6), 6M)
            };

            yield return new object[] { transactions, "lyft", quotes, lyftExpected };
            yield return new object[] { transactions, "msft", quotes, msftExpected };
        }

        [TestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void PositionValueTimeSeries(
            List<Transaction> transactions,
            string tickerSymbol,
            List<HistoricalPriceQuote> priceQuotes,
            List<DateValue> expected)
        {
            // Arrange
            var sut = new PositionValueTimeSeries(tickerSymbol, transactions, priceQuotes);

            // Act
            var actual = sut.ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
