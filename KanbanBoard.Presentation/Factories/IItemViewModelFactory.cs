using KanbanBoard.Presentation.ViewModels;

namespace KanbanBoard.Presentation.Factories
{
    public interface IItemViewModelFactory
    {
        ItemViewModel CreateItem(string title = null);

        ItemViewModel Load(string itemData);
    }
}