﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kanban.Core.Events;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Factories;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;

namespace KanbanBoard.Presentation.ViewModels
{
    public class ColumnViewModel : BaseCollectionItemViewModel
    {
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;

        private bool columnVisible;

        public ColumnViewModel(
            IItemViewModelFactory itemFactory,
            IDialogService dialogService,
            ILoggerFacade logger,
            IEventAggregator eventAggregator,
            Guid? id = null,
            string title = null,
            bool columnVisible = true,
            IEnumerable<ItemViewModel> items = null) 
            : base(id, title ?? Resources.Board_NewColumnName, eventAggregator)
        {
            // Loading a column.
            this.EventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteItem);

            this.dialogService = dialogService;
            this.logger = logger;

            this.columnVisible = columnVisible;

            if (items != null)
            {
                this.Items.AddRange(items);
            }
            
            this.AddItemCommand = new DelegateCommand(() => this.Items.Add(itemFactory.CreateItem()));
            this.DeleteColumnCommand = new DelegateCommand(() => this.EventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));

            this.Items.CollectionChanged += (o, e) => this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public ICommand AddItemCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        public bool ColumnVisible
        {
            get => columnVisible;
            set => SetProperty(ref columnVisible, value);
        }

        public bool Unchanged => this.Title == Resources.Board_NewColumnName && this.Items.Count <= 0;

        private void DeleteItem(Guid itemId)
        {
            var itemToDelete = this.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToDelete == null) return;

            if (itemToDelete.Unchanged
                || this.dialogService.ShowYesNo(Resources.Dialog_RemoveItem_Message, Resources.Dialog_RemoveItem_Title))
            {
                this.Items.Remove(itemToDelete);
            }

            this.logger.Log("Item deleted", Category.Debug, Priority.None);
        }

        public override string ToString()
        {
            var columnData = $"{this.Id + Properties.Resources.NewItemBreak + this.Title + Properties.Resources.NewItemBreak + this.ColumnVisible + Properties.Resources.NewItemBreak}";
            return this.Items.Aggregate(columnData, (current, item) => current + item + Properties.Resources.NewItemBreak);
        }
    }
}