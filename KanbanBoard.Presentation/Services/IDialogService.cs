namespace KanbanBoard.Presentation.Services
{
    public interface IDialogService
    {
        void ShowStartup();

        bool? ShowYesNo(string text, string caption);

        void ShowMessage(string text, string caption);

        string GetInput(string text, string caption);

        string SelectBoard();

        void ShowSettings();
    }
}