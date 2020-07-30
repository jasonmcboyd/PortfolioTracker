using PortfolioTracker.Wpf.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioTracker.Wpf.DataStructures
{
    public class PortfolioValueTimeSeries : IEnumerable<DateValue>
    {
        public PortfolioValueTimeSeries(IList<PositionValueTimeSeries> positionTrends)
        {
            PositionTrends = positionTrends;
        }

        private IList<PositionValueTimeSeries> PositionTrends { get; }

        public IEnumerator<DateValue> GetEnumerator() =>
            PositionTrends
            .Select(x => x as IEnumerable<DateValue>)
            .ToList()
            .ZipManyOrdered(
                x => x.Date.Date,
                (key, values) => new DateValue(key, values.Select(x => x.Value).Sum()))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
