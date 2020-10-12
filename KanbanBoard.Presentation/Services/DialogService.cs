using KanbanBoard.Presentation.Dialogs;

namespace KanbanBoard.Presentation.Services
{
    public class DialogService : IDialogService
    {
        public bool ShowYesNo(string text, string caption)
        {
            var dialog = new DialogBoxWindow();
            var dialogViewModel = new DialogBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Result;
        }

        public void Show(string text, string caption)
        {
            var dialog = new MessageBoxWindow();
            var dialogViewModel = new MessageBoxViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }

        public string GetInput(string text, string caption)
        {
            var dialog = new InputBoxWindow();
            var dialogViewModel = new InputBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Input;
        }

        public string SelectBoard()
        {
            var dialog = new BoardSelectorWindow();
            var dialogViewModel = new BoardSelectorWindowViewModel(this, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.BoardLocation;
        }
    }
}