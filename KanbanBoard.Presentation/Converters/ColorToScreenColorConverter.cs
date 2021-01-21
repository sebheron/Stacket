using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoard.Presentation.Converters
{
    public class ColorToScreenColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                if (parameter != null)
                {
                    var percent = System.Convert.ToInt32(parameter) / 100.0f;
                    var r = (byte)Math.Round(color.R * percent);
                    var g = (byte)Math.Round(color.G * percent);
                    var b = (byte)Math.Round(color.B * percent);
                    if (r > 255) r = 255;
                    if (g > 255) g = 255;
                    if (b > 255) b = 255;
                    if (r < 0) r = 0;
                    if (g < 0) g = 0;
                    if (b < 0) b = 0;
                    return new SolidColorBrush(Color.FromArgb(color.A, r, g, b));
                }
                return new SolidColorBrush(color);
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }

            return Colors.White;
        }
    }
}