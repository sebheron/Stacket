using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KanbanBoard {
   public class InputBoxViewModel : BindableBase {

      private Window dialogWindow;

      public string Caption { get; }
      public string Text { get; }
      public string Input { get; set; }

      public DelegateCommand OkButtonCommand => new DelegateCommand(OkButton, delegate () { return true; });
      public DelegateCommand CancelButtonCommand => new DelegateCommand(CancelButton, delegate () { return true; });

      public InputBoxViewModel(Window window, string text, string caption) {
         dialogWindow = window;
         Text = text;
         Caption = caption;
      }

      public void OkButton() {
         dialogWindow.Tag = Input;
         dialogWindow.DialogResult = true;
         dialogWindow.Close();
      }

      public void CancelButton() {
         dialogWindow.DialogResult = false;
         dialogWindow.Close();
      }
   }
}
