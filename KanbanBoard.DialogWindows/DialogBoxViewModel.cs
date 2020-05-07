using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KanbanBoard
{
    public class DialogBoxViewModel : BindableBase
    {

        private Window dialogWindow;

        public string Caption { get; }
        public string Text { get; }

        public DelegateCommand YesButtonCommand => new DelegateCommand(YesButton, delegate () { return true; });
        public DelegateCommand NoButtonCommand => new DelegateCommand(NoButton, delegate () { return true; });

        public DialogBoxViewModel(Window window, string text, string caption) {
            dialogWindow = window;
            Text = text;
            Caption = caption;
        }

        public void YesButton() {
            dialogWindow.DialogResult = true;
            dialogWindow.Close();
        }

        public void NoButton() {
            dialogWindow.DialogResult = false;
            dialogWindow.Close();
        }
    }
}
