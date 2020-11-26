using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private readonly IItemViewModelFactory itemFactory;

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
            this.itemFactory = itemFactory;

            this.EventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteItem);

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.dialogService = dialogService;
            this.logger = logger;

            this.columnVisible = columnVisible;

            this.Items.CollectionChanged += CollectionChanged;
            foreach (var item in items)
            {
                this.Items.Add(item);
            }

            this.AddItemCommand = new DelegateCommand(this.AddItem);
            this.DeleteColumnCommand = new DelegateCommand(() => this.EventAggregator.GetEvent<DeleteColumnEvent>().Publish(this.Id));

            this.Items.CollectionChanged += (o, e) => this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Request New")
            {
                this.AddItem();
            }
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public DragHandleBehavior DragHandler { get; }

        public ICommand AddItemCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        public bool ColumnVisible
        {
            get => columnVisible;
            set => SetProperty(ref columnVisible, value);
        }

        public bool Unchanged => this.Title == Resources.Board_NewColumnName && this.Items.Count <= 0;

        private void AddItem()
        {
            this.Items.Add(itemFactory.CreateItem());
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

        public override string ToString()
        {
            var columnData = $"{this.Id + Properties.Resources.NewItemBreak + this.Title + Properties.Resources.NewItemBreak + this.ColumnVisible + Properties.Resources.NewItemBreak}";
            return this.Items.Aggregate(columnData, (current, item) => current + item + Properties.Resources.NewItemBreak);
        }
    }
}