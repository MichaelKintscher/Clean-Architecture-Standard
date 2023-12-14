using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Windows.Converters
{
    /// <summary>
    /// Converts between a timespan and a timeonly struct.
    /// </summary>
    public class TimeOnlyToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeOnly timeOnlyValue = (TimeOnly)value;
            return new TimeSpan(timeOnlyValue.Hour, timeOnlyValue.Minute, timeOnlyValue.Second);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan == false)
            {
                return DependencyProperty.UnsetValue;
            }

            return TimeOnly.FromTimeSpan((TimeSpan)value);
        }
    }
}
