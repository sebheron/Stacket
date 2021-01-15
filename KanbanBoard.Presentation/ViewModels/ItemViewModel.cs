using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Kanban.Core.Events;
using KanbanBoard.Logic.Enums;
using KanbanBoard.Logic.Properties;
using Prism.Commands;
using Prism.Events;

namespace KanbanBoard.Presentation.ViewModels
{
    public class ItemViewModel : BaseCollectionItemViewModel
    {
        private string description;
        private bool descriptionVisible;
        private ItemTypes itemType;
        private Color itemColor;
        private bool newlyCreatedItem;
        private bool isItemEnabled;
        private bool isLocked;
        private bool optionsShown;
        private double itemWidth;

        public ItemViewModel(
            IEventAggregator eventAggregator,
            Guid? id = null,
            string title = null,
            string description = null,
            bool descriptionVisible = false,
            ItemTypes? itemType = null,
            bool isLocked = false)
            : base(id, title ?? Resources.Board_NewItemName, eventAggregator)
        {
            // Loading an item.
            this.Description = description;
            this.DescriptionVisible = descriptionVisible;
            this.ItemType = itemType ?? Settings.Default.LastItemType;
            this.IsLocked = isLocked;
            this.IsItemEnabled = true;
            this.OptionsShown = false;

            this.DeleteItemCommand = new DelegateCommand(() => this.EventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));
        }

        public ICommand DeleteItemCommand { get; }

        public string Description
        {
            get => this.description;
            set
            {
                if (this.description == value) return;

                this.SetProperty(ref this.description, value);
            }
        }

        public bool DescriptionVisible
        {
            get => this.descriptionVisible;
            set => this.SetProperty(ref this.descriptionVisible, value);
        }

        public ItemTypes ItemType
        {
            get => itemType;
            set
            {
                this.SetColor(value);
                this.SetProperty(ref this.itemType, value);
            }
        }

        public ItemTypes ItemTypeView
        {
            get => itemType;
            set
            {
                Settings.Default.LastItemType = value;
                this.ItemType = value;
            }
        }

        public Color ItemColor
        {
            get => this.itemColor;
            set => this.SetProperty(ref this.itemColor, value);
        }

        public bool NewlyCreatedItem
        {
            get => this.newlyCreatedItem;
            set => this.SetProperty(ref this.newlyCreatedItem, value);
        }

        public bool IsItemEnabled
        {
            get => this.isItemEnabled;
            set => this.SetProperty(ref this.isItemEnabled, value);
        }

        public bool IsLocked
        {
            get => this.isLocked;
            set
            {
                this.OptionsShown = false;
                this.SetProperty(ref this.isLocked, value);
            }
        }

        public bool OptionsShown
        {
            get => this.optionsShown;
            set => this.SetProperty(ref this.optionsShown, value);
        }

        public bool Unchanged => this.Title == Resources.Board_NewItemName && string.IsNullOrEmpty(this.Description);

        private void SetColor(ItemTypes item)
        {
            switch (item)
            {
                case ItemTypes.Bug:
                    this.ItemColor = Color.FromArgb(255, 255, 159, 26);
                    break;

                case ItemTypes.Investigation:
                    this.ItemColor = Color.FromArgb(255, 64, 86, 161);
                    break;

                case ItemTypes.Item:
                    this.ItemColor = Color.FromArgb(255, 147, 158, 196);
                    break;

                case ItemTypes.Parked:
                    this.ItemColor = Color.FromArgb(255, 241, 60, 31);
                    break;

                default:
                    this.ItemColor = Color.FromArgb(255, 147, 158, 196);
                    break;
            }
        }

        public override string ToString()
        {
            return this.Id + Properties.Resources.NewItemData + this.Title + Properties.Resources.NewItemData + this.Description
                   + Properties.Resources.NewItemData + this.ItemType + Properties.Resources.NewItemData + DateTime.Now.ToShortDateString() 
                   + Properties.Resources.NewItemData + this.DescriptionVisible + Properties.Resources.NewItemData + this.IsLocked;
        }
    }
}