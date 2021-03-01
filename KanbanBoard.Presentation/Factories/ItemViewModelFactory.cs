using System;
using KanbanBoard.Logic.Enums;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Factories
{
    public class ItemViewModelFactory : IItemViewModelFactory
    {
        private readonly IEventAggregator eventAggregator;

        public ItemViewModelFactory(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public ItemViewModel CreateItem(string title = null)
        {
            var item = new ItemViewModel(this.eventAggregator);

            if (!string.IsNullOrEmpty(title))
            {
                item.Title = title;
            }
            else
            {
                // New item so set the property respectively.
                item.NewlyCreatedItem = true;
            }

            return item;
        }

        public ItemViewModel Load(string itemData)
        {
            var parsedItemData = itemData.Split(new[] { Resources.NewItemData }, StringSplitOptions.None);
            var itemId = Guid.Parse(parsedItemData[0]);
            var itemTitle = parsedItemData[1];
            var itemDescription = parsedItemData[2];
            var itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), parsedItemData[3]);

            // Currently unused.
            // var itemDueDate = DateTime.Parse(parsedItemData[4]).Date;

            bool.TryParse(parsedItemData[5], out var itemDescriptionVisible);
            bool.TryParse(parsedItemData[6], out var itemLocked);

            return new ItemViewModel(this.eventAggregator, itemId, itemTitle, itemDescription, itemDescriptionVisible, itemType, itemLocked);
        }
    }
}