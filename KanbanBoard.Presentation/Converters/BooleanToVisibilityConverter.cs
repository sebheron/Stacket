using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KanbanBoard.Presentation.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
            }

            var visible = System.Convert.ToBoolean(value, culture);
            var state = System.Convert.ToInt32(parameter);
            if (state == 1)
            {
                visible = !visible;
            }
            else if (state == 2)
            {
                return visible ? Visibility.Visible : Visibility.Hidden;
            }
            else if (state == 3)
            {
                visible = !visible;
                return visible ? Visibility.Visible : Visibility.Hidden;
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }
    }
}