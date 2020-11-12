using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kanban.Core.Events;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Presentation.Factories;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class ColumnViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;
        private readonly IEventAggregator eventAggregator;

        private string title = Resources.Board_NewColumnName;
        private bool columnVisible = true;

        public ColumnViewModel(
            Guid id, 
            string title, 
            bool columnVisible, 
            IEnumerable<ItemViewModel> items, 
            IItemViewModelFactory itemFactory,
            IDialogService dialogService,
            ILoggerFacade logger,
            IEventAggregator eventAggregator) 
            : this(itemFactory, dialogService, logger, eventAggregator)
        {
            // Loading a column.
            this.Id = id;
            this.title = title;
            this.columnVisible = columnVisible;

            this.Items.AddRange(items);
        }

        public ColumnViewModel(
            IItemViewModelFactory itemFactory, 
            IDialogService dialogService,
            ILoggerFacade logger, 
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteItem);

            this.dialogService = dialogService;
            this.logger = logger;

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.AddItemCommand = new DelegateCommand(() => this.Items.Add(itemFactory.CreateItem()));
            this.DeleteColumnCommand = new DelegateCommand(() => this.eventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));

            this.Items.CollectionChanged += (o, e) => this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

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

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public DragHandleBehavior DragHandler { get; }

        public ICommand AddItemCommand { get; }
        public ICommand DeleteColumnCommand { get; }

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

        public bool ColumnVisible
        {
            get => columnVisible;
            set => SetProperty(ref columnVisible, value);
        }

        public bool Unchanged => this.Title == Resources.Board_NewColumnName && this.Items.Count <= 0;

        public override string ToString()
        {
            var columnData = $"{this.Id + Properties.Resources.NewItemBreak + this.Title + Properties.Resources.NewItemBreak + this.ColumnVisible + Properties.Resources.NewItemBreak}";
            return this.Items.Aggregate(columnData, (current, item) => current + item + Properties.Resources.NewItemBreak);
        }
    }
}