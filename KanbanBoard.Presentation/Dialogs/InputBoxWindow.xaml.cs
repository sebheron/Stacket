using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Dialogs
{
    /// <summary>
    ///     Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow(string text, string caption)
        {
            this.InitializeComponent();
            this.DataContext = new InputBoxViewModel(this, text, caption);
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