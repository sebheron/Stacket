using System;
using KanbanBoard.Logic.Enums;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Factories
{
    public class ItemViewModelFactory : IItemViewModelFactory
    {
        public ItemViewModel CreateItem(string title = null)
        {
            var item = new ItemViewModel();

            if (!string.IsNullOrEmpty(title))
            {
                item.Title = title;
            }

            return item;
        }

        public ItemViewModel Load(string itemData)
        {
            var parsedItemData = itemData.Split(new[] { Properties.Resources.NewItemData }, StringSplitOptions.None);
            var itemId = Guid.Parse(parsedItemData[0]);
            var itemTitle = parsedItemData[1];
            var itemDescription = parsedItemData[2];
            var itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), parsedItemData[3]);

            // Currently unused.
            // var itemDueDate = DateTime.Parse(parsedItemData[4]).Date;

            bool.TryParse(parsedItemData[5], out var itemDescriptionVisible);

            return new ItemViewModel(itemId, itemTitle, itemDescription, itemDescriptionVisible, itemType);
        }
    }
}