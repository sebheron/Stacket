using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace KanbanBoard.Presentation.Behaviors
{
    public class MoveableButtonBehavior : Behavior<UIElement>
    {
        private const int dragThreshold = 1;
        private const int snapThreshold = 15;

        private bool dragging;
        private Point prevPos;

        private double halfScreenWidth;
        private double halfButtonWidth;

        private double xPos;
        private TranslateTransform transform = new TranslateTransform();
        private Window parent;

        protected override void OnAttached()
        {
            if (this.AssociatedObject != null)
            {
                parent = Application.Current.MainWindow;
                AssociatedObject.RenderTransform = transform;
                xPos = transform.X = Properties.Settings.Default.TogglePosition;
                halfScreenWidth = SystemParameters.MaximizedPrimaryScreenWidth / 2;
                halfButtonWidth = ((ToggleButton)AssociatedObject).Width / 2;

                AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
                AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
                AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            }
        }

        protected override void OnDetaching()
        {
            if (this.AssociatedObject != null)
            {
                AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
                AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
                AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            }
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var delta = e.GetPosition(parent).X - prevPos.X;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(delta) > dragThreshold || dragging))
            {
                dragging = true;
                xPos += delta;
                if (Math.Abs(xPos) < snapThreshold)
                {
                    transform.X = 0;
                }
                else if (xPos > halfScreenWidth - halfButtonWidth)
                {
                    transform.X = halfScreenWidth - halfButtonWidth;
                }
                else if (xPos < -halfScreenWidth + halfButtonWidth)
                {
                    transform.X = -halfScreenWidth + halfButtonWidth;
                }
                else
                {
                    transform.X = xPos;
                }
            }
            prevPos = e.GetPosition(parent);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            prevPos = e.GetPosition(parent);
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dragging)
            {
                dragging = false;
                e.Handled = true;
                Properties.Settings.Default.TogglePosition = xPos = transform.X;
                Properties.Settings.Default.Save();
                ((ToggleButton)sender).ReleaseMouseCapture();
            }
        }
    }
}
