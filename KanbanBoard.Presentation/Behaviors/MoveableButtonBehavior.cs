﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace KanbanBoard.Presentation.Behaviors
{
    public class MoveableButtonBehavior : Behavior<UIElement>
    {
        private bool dragging;
        private Point prevPos;

        private double xPos;
        private TranslateTransform transform = new TranslateTransform();
        private Window parent;

        protected override void OnAttached()
        {
            if (this.AssociatedObject != null)
            {
                parent = Application.Current.MainWindow;
                AssociatedObject.RenderTransform = transform;
                xPos = transform.X;
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
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(delta) > 1 || dragging))
            {
                dragging = true;
                xPos += delta;
                if (Math.Abs(xPos) < 10)
                {
                    transform.X = 0;
                } else
                {
                    transform.X = xPos;
                }
                Console.WriteLine(transform.X);
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
                xPos = transform.X;
                ((ToggleButton)sender).ReleaseMouseCapture();
            }
        }
    }
}
