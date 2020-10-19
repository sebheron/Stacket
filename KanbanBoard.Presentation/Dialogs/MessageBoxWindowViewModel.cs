using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class MessageBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;

        public MessageBoxWindowViewModel(string text, string caption, Action closeDialog)
        {
            this.closeDialog = closeDialog;

            this.Text = text;
            this.Caption = caption;

            this.OkCommand = new DelegateCommand(this.CloseDialog);
        }

        public string Text { get; }
        public string Caption { get; }

        public ICommand OkCommand { get; }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }
    }
}