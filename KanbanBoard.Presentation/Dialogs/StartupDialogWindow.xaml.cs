using System.Windows;
using System.Windows.Input;

namespace KanbanBoard.Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for StartupDialogWindow.xaml
    /// </summary>
    public partial class StartupDialogWindow : Window
    {
        public StartupDialogWindow()
        {
            InitializeComponent();
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
