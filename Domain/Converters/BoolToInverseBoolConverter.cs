using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Converters
{
    /// <summary>
    /// Converts between a bool and the inverse of that bool.
    /// </summary>
    public class BoolToInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = (bool)value;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool convertedValue;

            if (value is bool == false)
            {
                return DependencyProperty.UnsetValue;
            }

            convertedValue = !(bool)value;

            return convertedValue;
        }
    }
}
