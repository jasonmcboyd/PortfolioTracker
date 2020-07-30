using PortfolioTracker.EF;
using PortfolioTracker.MarketProvider.Models;
using PortfolioTracker.Wpf.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioTracker.Wpf.DataStructures
{
    public class PositionValueTimeSeries : IEnumerable<DateValue>
    {
        public PositionValueTimeSeries(
            string tickerSymbol,
            IEnumerable<Transaction> transactions,
            IEnumerable<HistoricalPriceQuote> hitoricalQuotes)
        {
            TickerSymbol = tickerSymbol;
            Transactions = transactions;
            HistoricalQuotes = hitoricalQuotes;
        }

        public string TickerSymbol { get; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<HistoricalPriceQuote> HistoricalQuotes { get; set; }

        public IEnumerator<DateValue> GetEnumerator()
        {
            var accumulatedQuantity =
                Transactions
                .Where(x => x.TickerSymbol.Equals(TickerSymbol, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => (
                    x.TransactionDate.Date,
                    Quantity: x.Quantity * (x.TransactionType.Equals("B", StringComparison.InvariantCultureIgnoreCase) ? 1 : -1)
                    ))
                .Scan(
                    (Date: default(DateTime), Quantity: 0),
                    (a, x) => (x.Date, a.Quantity + x.Quantity));
            
            return
                HistoricalQuotes
                .ZipOrdered(
                    accumulatedQuantity,
                    l => l.Timestamp.Date,
                    r => r.Date,
                    (l, r) => (Quote: l, AccumulatedQuantity: r))
                .Where(x => x.Quote != null)
                .Select(x => (x.Quote.Timestamp.Date, x.Quote.Close, AccumulatedQuantity: x.AccumulatedQuantity.Quantity))
                .Scan(
                    (Date: default(DateTime), Close: 0M, Quantity: 0),
                    (a, x) => (x.Date, x.Close, x.AccumulatedQuantity == default ? a.Quantity : x.AccumulatedQuantity))
                .Select(x => new DateValue(x.Date, x.Quantity * x.Close))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
