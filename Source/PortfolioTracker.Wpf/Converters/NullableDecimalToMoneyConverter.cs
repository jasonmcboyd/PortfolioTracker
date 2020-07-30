using System;
using System.Globalization;
using System.Windows.Data;

namespace PortfolioTracker.Wpf.Converters
{
    [ValueConversion(typeof(decimal?), typeof(string))]
    class NullableDecimalToMoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = value as decimal?;

            return temp.HasValue ? temp.Value.ToString("C") : "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
