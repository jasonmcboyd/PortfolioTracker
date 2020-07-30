using System;
using System.Globalization;
using System.Windows.Data;

namespace PortfolioTracker.Wpf.Converters
{
    [ValueConversion(typeof(decimal?), typeof(string))]
    class PositiveNegativeToColorConverter : IValueConverter
    {
        public string PositiveColor { get; set; }
        public string ZeroColor { get; set; }
        public string NegativeColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = value as decimal?;

            if (temp == null || temp.Value == 0)
            {
                return ZeroColor;
            }
            else if(temp < 0)
            {
                return NegativeColor;
            }
            else
            {
                return PositiveColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
