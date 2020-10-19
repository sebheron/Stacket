using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Dialogs
{
    /// <summary>
    ///     Interaction logic for BoardSelectorWindow.xaml
    /// </summary>
    public partial class BoardSelectorWindow : Window
    {
        public BoardSelectorWindow()
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