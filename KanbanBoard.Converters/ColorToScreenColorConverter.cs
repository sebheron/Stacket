using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoard.Converters {
   public class ColorToScreenColorConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is Color color)
            return new SolidColorBrush(color);
         return new SolidColorBrush(Colors.White);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is SolidColorBrush brush)
            return brush.Color;
         return Colors.White;
      }
   }
}
