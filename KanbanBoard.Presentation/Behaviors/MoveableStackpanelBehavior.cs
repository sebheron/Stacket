using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using KanbanBoard.Logic.Properties;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace KanbanBoard.Presentation.Behaviors
{
    public class MoveableStackpanelBehavior : Behavior<UIElement>
    {
        private WrapPanel wrapPanel;

        private const int DragThreshold = 1;
        private const int SnapThreshold = 15;

        private bool dragging;
        private Point prevPos;

        private double halfScreenWidth;
        private double halfToggleWidth;

        private double xPos;
        private readonly TranslateTransform transform = new TranslateTransform();
        private Window parent;

        protected override void OnAttached()
        {
            if (this.AssociatedObject == null) return;
            this.parent = Application.Current.MainWindow;
            this.wrapPanel = (WrapPanel)this.AssociatedObject;
            this.wrapPanel.RenderTransform = this.transform;
            this.xPos = this.transform.X = Settings.Default.TogglePosition;
            this.halfScreenWidth = SystemParameters.MaximizedPrimaryScreenWidth / 2;
            this.wrapPanel.Margin = new Thickness(this.halfScreenWidth, 0, 0, 0);
            this.halfToggleWidth = 40;

            this.wrapPanel.PreviewMouseLeftButtonDown += this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.wrapPanel.PreviewMouseLeftButtonUp += this.AssociatedObject_PreviewMouseLeftButtonUp;
            this.wrapPanel.PreviewMouseMove += this.AssociatedObject_PreviewMouseMove;
        }

        protected override void OnDetaching()
        {
            if (this.AssociatedObject == null) return;
            this.wrapPanel.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.wrapPanel.PreviewMouseLeftButtonUp -= this.AssociatedObject_PreviewMouseLeftButtonUp;
            this.wrapPanel.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
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
                this.UpdatePanelPosition(sender, e);
            }
            this.prevPos = e.GetPosition(this.parent);
        }

        private void UpdatePanelPosition(object sender, EventArgs e)
        {
            if (this.xPos < -this.halfToggleWidth && this.xPos > -this.halfToggleWidth - SnapThreshold * 1.5)
            {
                this.transform.X = -SnapThreshold / 2 - this.halfToggleWidth;
            }
            else if (this.xPos > this.halfScreenWidth - this.wrapPanel.ActualWidth - SnapThreshold * 2)
            {
                this.transform.X = this.halfScreenWidth - this.wrapPanel.ActualWidth - SnapThreshold;
            }
            else if (this.xPos < -this.halfScreenWidth + SnapThreshold)
            {
                this.transform.X = -this.halfScreenWidth;
            }
            else
            {
                this.transform.X = this.xPos;
            }
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
                ReleaseAllChildren((WrapPanel)sender);
            }
        }

        private void ReleaseAllChildren(Panel panel)
        {
            foreach (UIElement child in panel.Children)
            {
                if (child is ButtonBase childButton && childButton.IsMouseCaptured)
                {
                    childButton.ReleaseMouseCapture();
                }
                else if (child is Panel childPanel)
                {
                    ReleaseAllChildren(childPanel);
                }
            }
        }
    }
}