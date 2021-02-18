using KanbanBoard.Presentation.Dialogs;

namespace KanbanBoard.Presentation.Services
{
    public class DialogService : IDialogService
    {
        private readonly IRegistryService registryService;
        private readonly IStringLogger logger;

        public DialogService(IRegistryService registryService, IStringLogger logger)
        {
            this.registryService = registryService;
            this.logger = logger;
        }

        public bool? ShowYesNo(string text, string caption)
        {
            this.logger.Log("Requested yes no dialog");
            var dialog = new DialogBoxWindow();
            var dialogViewModel = new DialogBoxWindowViewModel(text, caption, dialog.Close, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Result;
        }

        public void ShowMessage(string text, string caption)
        {
            this.logger.Log("Requested message dialog");
            var dialog = new MessageBoxWindow();
            var dialogViewModel = new MessageBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }

        public string GetInput(string text, string caption)
        {
            this.logger.Log("Requested input dialog");
            var dialog = new InputBoxWindow();
            var dialogViewModel = new InputBoxWindowViewModel(text, caption, dialog.Close, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Input;
        }

        public string SelectBoard()
        {
            this.logger.Log("Requested board window");
            var dialog = new BoardSelectorWindow();
            var dialogViewModel = new BoardSelectorWindowViewModel(this, this.logger, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.BoardLocation;
        }

        public void ShowSettings()
        {
            this.logger.Log("Requested settings window");
            var dialog = new SettingsWindow();
            var dialogViewModel = new SettingsWindowViewModel(dialog.Close, this.registryService, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }
    }
}