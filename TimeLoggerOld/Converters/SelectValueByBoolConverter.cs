using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TimeLoggerOld
{
    public sealed class SelectValueByBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 3)
                return null;
            bool checker = false;
            if (values[0] is bool)
                checker = (bool)values[0];
            else if (values[0] is bool?)
            {
                var nullChecker = (bool?)values[0];
                checker = nullChecker.HasValue && nullChecker.Value;
            }
            return (checker ? values[1] : values[2]).ToString();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("SelectValueByBoolConverter is a one way converter.");
        }
    }
}
