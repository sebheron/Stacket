using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace KanbanBoard.Presentation.Behaviors
{
    public class TextBoxFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            base.OnDetaching();
        }

        protected virtual void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && e.Key == Key.Return && e.Key == Key.Enter)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}