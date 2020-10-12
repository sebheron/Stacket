using KanbanBoard.Presentation.Dialogs;

namespace KanbanBoard.Presentation.Services
{
    public class DialogBoxService
    {
        public static bool ShowYesNo(string text, string caption)
        {
            // Get viewmodel from DI here and assign parameters
            // Get viewmodel from DI here and assign dialog.Close through parameters?
            var dialog = new DialogBoxWindow();
            var dialogViewModel = new DialogBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Result;
        }

        public static void Show(string text, string caption)
        {
            var dialog = new MessageBoxWindow();
            var dialogViewModel = new MessageBoxViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }

        public static string GetInput(string text, string caption)
        {
            var dialog = new InputBoxWindow();
            var dialogViewModel = new InputBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Input;
        }

        public static string SelectBoard()
        {
            var dialog = new BoardSelectorWindow();
            var dialogViewModel = new BoardSelectorWindowViewModel(dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.BoardLocation;
        }
    }
}