using KanbanBoard.Presentation.ViewModels;

namespace KanbanBoard.Presentation.Factories
{
    public interface IColumnViewModelFactory
    {
        ColumnViewModel CreateColumn(string title = null);

        ColumnViewModel Load(string columnData);
    }
}