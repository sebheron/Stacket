using System;
using System.Windows.Input;
using System.Windows.Media;
using Kanban.Core.Events;
using KanbanBoard.Logic.Enums;
using KanbanBoard.Logic.Properties;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class ItemViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private string title = Resources.Board_NewItemName;
        private string description;
        private bool descriptionVisible;
        private ItemTypes itemType = Settings.Default.LastItemType;
        private Color itemColor;

        public ItemViewModel(
            Guid id,
            string title,
            string description,
            bool descriptionVisible,
            ItemTypes itemType,
            IEventAggregator eventAggregator)
            : this(eventAggregator)
        {
            // Loading an item.
            this.Id = id;
            this.title = title;
            this.description = description;
            this.DescriptionVisible = descriptionVisible;
            this.ItemType = itemType;
        }

        public ItemViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.DeleteItemCommand = new DelegateCommand(() => this.eventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));

            this.SetColor(this.ItemType);
        }

        public ICommand DeleteItemCommand { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public string Title
        {
            get => title;
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                this.SetProperty(ref title, value);
                this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public string Description
        {
            get => this.description;
            set
            {
                if (this.description == value) return;

                this.SetProperty(ref this.description, value);
                this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
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
                   + Properties.Resources.NewItemData + this.DescriptionVisible;
        }
    }
}