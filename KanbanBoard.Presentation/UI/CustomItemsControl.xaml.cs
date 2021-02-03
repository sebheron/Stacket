using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KanbanBoard.Presentation.UI
{
    /// <summary>
    /// Interaction logic for CustomUserControl.xaml
    /// </summary>
    public partial class CustomItemsControl : ItemsControl
    {
        public CustomItemsControl()
        {
            InitializeComponent();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Move)
            {
                Mouse.SetCursor((Cursor)this.Resources["GrabCursor"]);
            }
            e.Handled = true;
        }
    }
}
