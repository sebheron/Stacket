using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.DialogWindows
{
    public class InputBoxViewModel : BindableBase
    {
        private readonly Window dialogWindow;

        public InputBoxViewModel(Window window, string text, string caption)
        {
            this.dialogWindow = window;
            this.Text = text;
            this.Caption = caption;

            this.OkButtonCommand = new DelegateCommand(OkButton, () => true);
            this.EnterPressedCommand = new DelegateCommand<string>(EnterPressed, arg => true);
            this.CancelButtonCommand = new DelegateCommand(CancelButton, () => true);
        }

        public string Caption { get; }
        public string Text { get; }
        public string Input { get; set; }

        public ICommand OkButtonCommand { get; }
        public ICommand EnterPressedCommand { get; }
        public ICommand CancelButtonCommand { get; }

        public void OkButton()
        {
            this.dialogWindow.Tag = Input;
            this.dialogWindow.DialogResult = true;
            this.dialogWindow.Close();
        }

        public void EnterPressed(string input)
        {
            this.dialogWindow.Tag = input;
            this.dialogWindow.DialogResult = true;
            this.dialogWindow.Close();
        }

        public void CancelButton()
        {
            this.dialogWindow.DialogResult = false;
            this.dialogWindow.Close();
        }
    }
}