using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoard.DialogWindows {
   public class DialogBoxService {
      public static bool? Show(string text) {
         DialogBoxWindow dialog = new DialogBoxWindow(text, "Kanban Board");
         dialog.ShowDialog();
         if (dialog.DialogResult == null) {
            dialog.DialogResult = false;
         }
         return dialog.DialogResult;
      }

      public static bool? Show(string text, string caption) {
         DialogBoxWindow dialog = new DialogBoxWindow(text, caption);
         dialog.ShowDialog();
         if (dialog.DialogResult == null) {
            dialog.DialogResult = false;
         }
         return dialog.DialogResult;
      }
   }
}
