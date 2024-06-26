using System;
using System.Windows.Input;
using Prism.Commands;
using KanbanBoard.Presentation.Services;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class DialogBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IStringLogger logger;

        public DialogBoxWindowViewModel(string text, string caption, Action closeDialog, IStringLogger logger)
        {
            this.closeDialog = closeDialog;
            this.logger = logger;

            this.Text = text;
            this.Caption = caption;

            this.YesButtonCommand = new DelegateCommand(this.YesButton);
            this.NoButtonCommand = new DelegateCommand(this.NoButton);
            this.CancelButtonCommand = new DelegateCommand(this.CancelButton);
        }

        public bool? Result { get; private set; }

        public string Caption { get; }
        public string Text { get; }

        public ICommand YesButtonCommand { get; }
        public ICommand NoButtonCommand { get; }
        public ICommand CancelButtonCommand { get; }

        public void YesButton()
        {
            this.Result = true;
            this.logger.Log("Yes selected");
            this.closeDialog.Invoke();
        }

        private void NoButton()
        {
            this.Result = false;
            this.logger.Log("No selected");
            this.closeDialog.Invoke();
        }

        private void CancelButton()
        {
            this.logger.Log("Cancel selected");
            this.closeDialog.Invoke();
        }
    }
}