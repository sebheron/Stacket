using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.DialogWindows
{
    /// <summary>
    ///     Interaction logic for InputBox.xaml
    /// </summary>
    public partial class DialogBoxWindow : Window
    {
        public DialogBoxWindow(string text, string caption)
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