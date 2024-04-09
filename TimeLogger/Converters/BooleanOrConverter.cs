using System;
using System.Linq;
using System.Windows.Data;

namespace TimeLogger.Converters
{
    public class BooleanOrConverter : IMultiValueConverter
    {
        public object? Convert(object?[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values
                .Select(value => (value as bool?) ?? false)
                .Any(val => val);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("BooleanOrConverter is a one way converter.");
        }
    }
}
