using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class DialogBoxWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;

        public DialogBoxWindowViewModel(string text, string caption, Action closeDialog)
        {
            this.closeDialog = closeDialog;

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
            this.Close();
        }

        private void Close()
        {
            this.closeDialog.Invoke();
        }
    }
}