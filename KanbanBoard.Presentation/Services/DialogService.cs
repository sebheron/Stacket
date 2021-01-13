using KanbanBoard.Presentation.Dialogs;
using Prism.Events;
using Prism.Logging;

namespace KanbanBoard.Presentation.Services
{
    public class DialogService : IDialogService
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IRegistryService registryService;
        private readonly ILoggerFacade logger;
        private readonly IFTPService ftpService;

        public DialogService(IEventAggregator eventAggregator, IRegistryService registryService, ILoggerFacade logger, IFTPService ftpService)
        {
            this.registryService = registryService;
            this.logger = logger;
            this.ftpService = ftpService;
            this.eventAggregator = eventAggregator;
        }

        public bool? ShowYesNo(string text, string caption)
        {
            this.logger.Log("Requested yes no dialog", Category.Debug, Priority.None);
            var dialog = new DialogBoxWindow();
            var dialogViewModel = new DialogBoxWindowViewModel(text, caption, dialog.Close, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Result;
        }

        public void ShowMessage(string text, string caption)
        {
            this.logger.Log("Requested message dialog", Category.Debug, Priority.None);
            var dialog = new MessageBoxWindow();
            var dialogViewModel = new MessageBoxWindowViewModel(text, caption, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }

        public string GetInput(string text, string caption)
        {
            this.logger.Log("Requested input dialog", Category.Debug, Priority.None);
            var dialog = new InputBoxWindow();
            var dialogViewModel = new InputBoxWindowViewModel(text, caption, dialog.Close, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.Input;
        }

        public string SelectBoard()
        {
            this.logger.Log("Requested board window", Category.Debug, Priority.None);
            var dialog = new BoardSelectorWindow();
            var dialogViewModel = new BoardSelectorWindowViewModel(this.eventAggregator, this, this.logger, this.ftpService, dialog.Close);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();

            return dialogViewModel.BoardLocation;
        }

        public void ShowSettings()
        {
            this.logger.Log("Requested settings window", Category.Debug, Priority.None);
            var dialog = new SettingsWindow();
            var dialogViewModel = new SettingsWindowViewModel(dialog.Close, this.registryService, this.logger);
            dialog.DataContext = dialogViewModel;

            dialog.ShowDialog();
        }
    }
}