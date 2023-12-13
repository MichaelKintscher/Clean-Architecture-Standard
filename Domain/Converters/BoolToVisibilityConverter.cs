using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitectureWindows.Converters
{
    /// <summary>
    /// Converts between a bool and a visibility value. The visbility values TRUE and FALSE map to can be configured.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// The visibility value to convert TRUE to/from.
        /// </summary>
        public Visibility TrueVisibility { get; set; }

        /// <summary>
        /// The visibility value to convert FALSE to/from.
        /// </summary>
        public Visibility FalseVisibility { get; set; }

        public BoolToVisibilityConverter()
        {
            this.TrueVisibility = Visibility.Visible;
            this.TrueVisibility = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = (bool)value;
            return boolValue ? this.TrueVisibility : this.FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool convertedValue;

            if (value is Visibility == false)
            {
                return DependencyProperty.UnsetValue;
            }

            if ((Visibility)value == this.TrueVisibility)
            {
                convertedValue = true;
            }
            else
            {
                convertedValue = false;
            }

            return convertedValue;
        }
    }
}
