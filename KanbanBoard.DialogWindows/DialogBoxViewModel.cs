using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.DialogWindows
{
    public class DialogBoxViewModel : BindableBase
    {
        private readonly Window dialogWindow;

        public DialogBoxViewModel(Window window, string text, string caption)
        {
            this.dialogWindow = window;
            this.Text = text;
            this.Caption = caption;

            this.YesButtonCommand = new DelegateCommand(YesButton, () => true);
            this.NoButtonCommand = new DelegateCommand(NoButton, () => true);
        }

        public string Caption { get; }
        public string Text { get; }

        public ICommand YesButtonCommand { get; }
        public ICommand NoButtonCommand { get; }

        public void YesButton()
        {
            this.dialogWindow.DialogResult = true;
            this.dialogWindow.Close();
        }

        public void NoButton()
        {
            this.dialogWindow.DialogResult = false;
            this.dialogWindow.Close();
        }
    }
}