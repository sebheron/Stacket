using System;
using System.Windows.Input;
using Prism.Commands;
using KanbanBoard.Presentation.Services;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class InputBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IStringLogger logger;

        public InputBoxWindowViewModel(string text, string caption, Action closeDialog, IStringLogger logger)
        {
            this.closeDialog = closeDialog;
            this.logger = logger;
            this.Text = text;
            this.Caption = caption;

            this.OkCommand = new DelegateCommand(this.Ok);
            this.CancelCommand = new DelegateCommand(this.Cancel);
        }

        public string Input { get; set; }
        public string Caption { get; }
        public string Text { get; }

        public ICommand CancelCommand { get; }
        public ICommand OkCommand { get; }

        private void Cancel()
        {
            this.logger.Log("Cancel selected");
            this.Input = string.Empty;
            this.closeDialog.Invoke();
        }

        private void Ok()
        {
            this.logger.Log("Ok selected");
            this.closeDialog.Invoke();
        }
    }
}