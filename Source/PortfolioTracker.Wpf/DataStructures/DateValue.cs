using System;

namespace PortfolioTracker.Wpf.DataStructures
{
    public struct DateValue
    {
        public DateValue(DateTime date, decimal value)
        {
            Date = date.Date;
            Value = value;
        }
        public DateTime Date { get; }
        public decimal Value { get; }
    }
}
