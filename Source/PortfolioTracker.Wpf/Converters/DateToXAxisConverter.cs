using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PortfolioTracker.Wpf.Converters
{
    [ValueConversion(typeof(DateTime), typeof(long))]
    class DateToXAxisConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = value as DateTime?;

            if (temp == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return temp.Value.Ticks / TimeSpan.FromHours(1).Ticks;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = value as long?;

            if (temp == null)
            {
                return default(DateTime);
            }

            return new DateTime(temp.Value * TimeSpan.FromHours(1).Ticks);
        }
    }
}
