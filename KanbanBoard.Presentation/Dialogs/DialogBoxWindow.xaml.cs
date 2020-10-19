using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Dialogs
{
    /// <summary>
    ///     Interaction logic for InputBox.xaml
    /// </summary>
    public partial class DialogBoxWindow : Window
    {
        public DialogBoxWindow()
        {
            this.InitializeComponent();
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