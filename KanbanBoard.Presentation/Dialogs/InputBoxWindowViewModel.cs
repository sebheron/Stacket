using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class InputBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;

        public InputBoxWindowViewModel(string text, string caption, Action closeDialog)
        {
            this.closeDialog = closeDialog;
            this.Text = text;
            this.Caption = caption;

            this.CloseDialogCommand = new DelegateCommand(this.CloseDialog);
            this.CancelCommand = new DelegateCommand(this.Cancel);
        }

        public string Input { get; set; }
        public string Caption { get; }
        public string Text { get; }

        public ICommand CancelCommand { get; }
        public ICommand CloseDialogCommand { get; }

        private void Cancel()
        {
            this.Input = string.Empty;
        }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }
    }
}