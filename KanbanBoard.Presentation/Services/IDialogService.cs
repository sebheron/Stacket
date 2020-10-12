namespace KanbanBoard.Presentation.Services
{
    public interface IDialogService
    {
        bool ShowYesNo(string text, string caption);

        void Show(string text, string caption);

        string GetInput(string text, string caption);

        string SelectBoard();
    }
}
