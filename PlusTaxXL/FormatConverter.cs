using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace PlusTaxXL
{
    class FormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"In the converter: {value}");
            return string.Format($"{0:N2}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
