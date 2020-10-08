using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KanbanBoard.Presentation.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool InvertVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
            }

            var visible = System.Convert.ToBoolean(value, culture);
            if (this.InvertVisibility)
            {
                visible = !visible;
            }
                
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }
    }
}