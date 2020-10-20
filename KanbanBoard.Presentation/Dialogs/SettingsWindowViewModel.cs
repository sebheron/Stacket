using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    class SettingsWindowViewModel
    {
        private readonly Action closeDialog;

        public SettingsWindowViewModel(string text, string caption, Action closeDialog) {
            this.closeDialog = closeDialog;

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.AcceptCommand = new DelegateCommand(this.Accept);
        }

        public ICommand CancelCommand { get; }
        public ICommand AcceptCommand { get; }

        private void Cancel() {
            this.closeDialog.Invoke();
        }

        private void Accept() {
            this.closeDialog.Invoke();
        }
    }
}
