using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Views
{
    /// <summary>
    ///     Interaction logic for BoardSelectorWindow.xaml
    /// </summary>
    public partial class BoardSelectorWindow : Window
    {
        public BoardSelectorWindow(string currentBoard)
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