using KanbanBoard.Presentation.Dialogs;

namespace KanbanBoard.Presentation.Services
{
    public class DialogBoxService
    {
        public static bool ShowYesNo(string text, string caption)
        {
            var dialog = new DialogBoxWindow(text, caption);
            dialog.ShowDialog();

            return dialog.DialogResult != null && dialog.DialogResult.Value;
        }

        public static bool Show(string text, string caption)
        {
            var dialog = new MessageBoxWindow(text, caption);
            dialog.ShowDialog();

            return true;
        }

        public static string GetInput(string text, string caption)
        {
            var dialog = new InputBoxWindow(text, caption);
            dialog.ShowDialog();

            if (dialog.DialogResult == null || !dialog.DialogResult.Value || dialog.Tag == null) return string.Empty;

            return (string) dialog.Tag;
        }

        public static string SelectBoard(string currentBoard)
        {
            var dialog = new BoardSelectorWindow(currentBoard);
            dialog.ShowDialog();

            if (dialog.DialogResult == null || dialog.DialogResult.Value == false) return string.Empty;

            return dialog.Tag.ToString();
        }
    }
}