using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoard.DialogWindows
{
    public class DialogBoxService
    {
        public static bool ShowYesNo(string text, string caption) {
            DialogBoxWindow dialog = new DialogBoxWindow(text, caption);
            dialog.ShowDialog();
            if (dialog.DialogResult == null) {
                return false;
            }
            return dialog.DialogResult.Value;
        }

        public static bool Show(string text, string caption) {
            MessageBoxWindow dialog = new MessageBoxWindow(text, caption);
            dialog.ShowDialog();
            return true;
        }

        public static string GetInput(string text, string caption) {
            InputBoxWindow dialog = new InputBoxWindow(text, caption);
            dialog.ShowDialog();
            if (dialog.DialogResult == null || !dialog.DialogResult.Value || dialog.Tag == null) {
                return string.Empty;
            }
            return (string)dialog.Tag;
        }

        public static string SelectBoard(string currentBoard) {
            BoardSelectorWindow dialog = new BoardSelectorWindow(currentBoard);
            dialog.ShowDialog();
            if (dialog.DialogResult == null || dialog.DialogResult.Value == false) {
                return string.Empty;
            }
            return dialog.Tag.ToString();
        }
    }
}
