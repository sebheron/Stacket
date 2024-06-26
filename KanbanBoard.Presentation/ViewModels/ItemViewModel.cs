﻿using System;
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
        private bool isLocked;
        private bool optionsShown;

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
            if (eventAggregator == null) return;

            // Loading an item.
            this.Description = description;
            this.DescriptionVisible = descriptionVisible;
            this.ItemType = itemType ?? Settings.Default.LastItemType;
            this.IsLocked = isLocked;
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
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public bool DescriptionVisible
        {
            get => this.descriptionVisible;
            set
            {
                this.SetProperty(ref this.descriptionVisible, value);
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public ItemTypes ItemType
        {
            get => itemType;
            set
            {
                this.SetColor(value);
                this.SetProperty(ref this.itemType, value);
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
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

        public bool IsLocked
        {
            get => this.isLocked;
            set
            {
                this.SetProperty(ref this.isLocked, value);
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public bool OptionsShown
        {
            get => this.optionsShown;
            set => this.SetProperty(ref this.optionsShown, value);
        }

        public double Width => (SystemParameters.MaximizedPrimaryScreenWidth - 120) / 5;

        private void SetColor(ItemTypes item)
        {
            switch (item)
            {
                case ItemTypes.Bug:
                    this.ItemColor = Color.FromArgb(255, 255, 159, 26);
                    break;

                case ItemTypes.Investigation:
                    this.ItemColor = Color.FromArgb(255, 82, 109, 204);
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
            return this.Id + Resources.NewItemData + this.Title + Resources.NewItemData + this.Description
                   + Resources.NewItemData + this.ItemType + Resources.NewItemData + DateTime.Now.ToShortDateString()
                   + Resources.NewItemData + this.DescriptionVisible + Resources.NewItemData + this.IsLocked;
        }
    }
}