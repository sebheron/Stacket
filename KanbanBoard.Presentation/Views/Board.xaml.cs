using System.Windows;

namespace KanbanBoard.Presentation.Views
{
    public partial class Board : Window
    {
        public Board()
        {
            var boardViewModel = new BoardViewModel();
            this.Closing += boardViewModel.OnClosing;
            this.DataContext = boardViewModel;

            this.InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}