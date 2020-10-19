﻿namespace KanbanBoard.Presentation.Services
{
    public interface IDialogService
    {
        bool ShowYesNo(string text, string caption);

        void ShowMessage(string text, string caption);

        string GetInput(string text, string caption);

        string SelectBoard();

        void ShowSettings();
    }
}