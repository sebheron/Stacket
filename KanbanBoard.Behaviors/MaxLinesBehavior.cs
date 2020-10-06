using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace KanbanBoard.Behaviors
{
    public class MaxLinesBehavior : Behavior<TextBlock>
    {
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.RegisterAttached(
                "MaxLines",
                typeof(int),
                typeof(MaxLinesBehavior),
                new PropertyMetadata(default(int), OnMaxLinesPropertyChangedCallback));

        public static readonly DependencyProperty MinLinesProperty =
            DependencyProperty.RegisterAttached(
                "MinLines",
                typeof(int),
                typeof(MaxLinesBehavior),
                new PropertyMetadata(default(int), OnMinLinesPropertyChangedCallback));

        private TextBlock TextBlock => AssociatedObject;

        public static void SetMaxLines(DependencyObject element, int value)
        {
            element.SetValue(MaxLinesProperty, value);
        }

        public static int GetMaxLines(DependencyObject element)
        {
            return (int) element.GetValue(MaxLinesProperty);
        }

        private static void OnMaxLinesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock element)
            {
                element.MaxHeight = getLineHeight(element) * GetMaxLines(element);
            }
        }

        public static void SetMinLines(DependencyObject element, int value)
        {
            element.SetValue(MinLinesProperty, value);
        }

        public static int GetMinLines(DependencyObject element)
        {
            return (int) element.GetValue(MinLinesProperty);
        }

        private static void OnMinLinesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as TextBlock;
            element.MinHeight = getLineHeight(element) * GetMinLines(element);
        }

        private static double getLineHeight(TextBlock textBlock)
        {
            var lineHeight = textBlock.LineHeight;
            if (double.IsNaN(lineHeight))
                lineHeight = Math.Ceiling(textBlock.FontSize * textBlock.FontFamily.LineSpacing);
            return lineHeight;
        }
    }
}