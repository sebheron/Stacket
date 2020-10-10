using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Dialogs
{
    /// <summary>
    ///     Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow(string text, string caption)
        {
            this.InitializeComponent();
            this.DataContext = new DialogBoxViewModel(this, text, caption);
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}