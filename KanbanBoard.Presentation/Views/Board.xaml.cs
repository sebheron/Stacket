using System.Windows;

namespace KanbanBoard.Presentation.Views
{
    public partial class Board : Window
    {
        public Board()
        {
            this.InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}