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
            if (eventAggregator == null) return;

            // Loading a column.
            this.EventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteItem);

            this.DragHandler = new DragHandleBehavior(this.EventAggregator);
            this.DragHandler.DragStarted += (d) => this.RaisePropertyChanged(nameof(this.DragHandler));
            this.DropHandler = new DropHandleBehavior(new ItemViewModel(null));
            this.HeaderDropHandler = new HeaderDropHandleBehavior(this.Items);

            this.dialogService = dialogService;
            this.logger = logger;

            this.columnVisible = columnVisible;

            if (items != null)
            {
                this.Items.AddRange(items);
            }

            this.AddItemCommand = new DelegateCommand(() => this.AddItem(itemFactory));
            this.DeleteColumnCommand = new DelegateCommand(() => this.EventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public DragHandleBehavior DragHandler { get; }
        public DropHandleBehavior DropHandler { get; }
        public HeaderDropHandleBehavior HeaderDropHandler { get; }

        public ICommand AddItemCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        public bool ColumnVisible
        {
            get => columnVisible;
            set
            {
                SetProperty(ref columnVisible, value);
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public void AddItem(IItemViewModelFactory itemFactory)
        {
            this.Items.Add(itemFactory.CreateItem());
            this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        private void DeleteItem(Guid itemId)
        {
            var itemToDelete = this.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToDelete == null) return;

            this.Items.Remove(itemToDelete);

            this.logger.Log("Item deleted", Category.Debug, Priority.None);
            this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        public override string ToString()
        {
            var columnData = $"{this.Id + Properties.Resources.NewItemBreak + this.Title + Properties.Resources.NewItemBreak + this.ColumnVisible + Properties.Resources.NewItemBreak}";
            return this.Items.Aggregate(columnData, (current, item) => current + item + Properties.Resources.NewItemBreak);
        }
    }
}