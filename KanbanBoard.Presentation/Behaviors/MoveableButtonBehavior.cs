using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using KanbanBoard.Logic.Properties;

namespace KanbanBoard.Presentation.Behaviors
{
    public class MoveableButtonBehavior : Behavior<UIElement>
    {
        private const int DragThreshold = 1;
        private const int SnapThreshold = 15;

        private bool dragging;
        private Point prevPos;

        private double halfScreenWidth;
        private double halfButtonWidth;

        private double xPos;
        private readonly TranslateTransform transform = new TranslateTransform();
        private Window parent;

        protected override void OnAttached()
        {
            if (this.AssociatedObject == null) return;
            this.parent = Application.Current.MainWindow;
            this.AssociatedObject.RenderTransform = this.transform;
            this.xPos = this.transform.X = Settings.Default.TogglePosition;
            this.halfScreenWidth = SystemParameters.MaximizedPrimaryScreenWidth / 2;
            this.halfButtonWidth = ((ToggleButton)this.AssociatedObject).Width / 2;

            this.AssociatedObject.PreviewMouseLeftButtonDown += this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseLeftButtonUp += this.AssociatedObject_PreviewMouseLeftButtonUp;
            this.AssociatedObject.PreviewMouseMove += this.AssociatedObject_PreviewMouseMove;
        }

        protected override void OnDetaching()
        {
            if (this.AssociatedObject == null) return;
            this.AssociatedObject.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseLeftButtonUp -= this.AssociatedObject_PreviewMouseLeftButtonUp;
            this.AssociatedObject.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var delta = e.GetPosition(this.parent).X - this.prevPos.X;

            if (!Settings.Default.LockToggle
                && e.LeftButton == MouseButtonState.Pressed
                && (Math.Abs(delta) > DragThreshold || this.dragging))
            {
                this.dragging = true;
                this.xPos += delta;

                if (Math.Abs(xPos) < SnapThreshold)
                {
                    this.transform.X = 0;
                }
                else if (this.xPos > this.halfScreenWidth - this.halfButtonWidth)
                {
                    this.transform.X = this.halfScreenWidth - this.halfButtonWidth;
                }
                else if (this.xPos < -this.halfScreenWidth + this.halfButtonWidth)
                {
                    this.transform.X = -this.halfScreenWidth + this.halfButtonWidth;
                }
                else
                {
                    this.transform.X = this.xPos;
                }
            }
            this.prevPos = e.GetPosition(this.parent);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.dragging = false;
            this.prevPos = e.GetPosition(this.parent);
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;
                e.Handled = true;
                Settings.Default.TogglePosition = this.xPos = this.transform.X;
                ((ToggleButton)sender).ReleaseMouseCapture();
            }
        }
    }
}