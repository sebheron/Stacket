using KanbanBoard.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Behaviors
{
    public class PrimeTextBoxFocusBehavior : TextBoxFocusBehavior
    { 
        private UserControl parentControl;

        protected override void OnAttached()
        {
            this.parentControl = this.FindParentOfType((FrameworkElement)this.AssociatedObject.Parent);
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        protected void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (parentControl != null && parentControl.DataContext is BaseCollectionItemViewModel vm && vm.New)
            {
                vm.New = false;
                this.AssociatedObject.Focus();
            }
        }

        protected override void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox && e.Key == Key.Tab && parentControl.DataContext is BaseCollectionItemViewModel vm)
            {
                vm.RequestNew();
            }
            base.AssociatedObject_KeyDown(sender, e);
        }

        public UserControl FindParentOfType(FrameworkElement element)
        {
            if (element == null) return default(UserControl);
            if (element is UserControl parentOfType) return parentOfType;
            return this.FindParentOfType((FrameworkElement)element.Parent);
        }
    }
}
