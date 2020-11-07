using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class DialogBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly ILoggerFacade logger;

        public DialogBoxWindowViewModel(string text, string caption, Action closeDialog, ILoggerFacade logger)
        {
            this.closeDialog = closeDialog;
            this.logger = logger;

            this.Text = text;
            this.Caption = caption;

            this.YesButtonCommand = new DelegateCommand(this.YesButton);
            this.NoButtonCommand = new DelegateCommand(this.Close);
        }

        public bool Result { get; private set; }

        public string Caption { get; }
        public string Text { get; }

        public ICommand YesButtonCommand { get; }
        public ICommand NoButtonCommand { get; }

        public void YesButton()
        {
            this.Result = true;
            this.logger.Log("Yes selected", Category.Debug, Priority.None);
            this.closeDialog.Invoke();
        }

        private void Close()
        {
            this.logger.Log("No selected", Category.Debug, Priority.None);
            this.closeDialog.Invoke();
        }
    }
}