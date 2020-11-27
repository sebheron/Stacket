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
        private readonly ILoggerFacade logger;

        private bool columnVisible;

        public ColumnViewModel(
            IItemViewModelFactory itemFactory,
            ILoggerFacade logger,
            Guid? id = null,
            string title = null,
            bool columnVisible = true,
            IList<ItemViewModel> items = null)
            : base(id, title ?? Resources.Board_NewColumnName)
        {
            // Loading a column.

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.logger = logger;

            this.columnVisible = columnVisible;

            for (int i = 0; i < (items?.Count ?? 0); i++)
            {
                this.Items.Add(items[i]);
            }

            this.AddItemCommand = new DelegateCommand(() => this.Items.Add(itemFactory.CreateItem()));
            this.DeleteColumnCommand = new DelegateCommand(() => this.RaisePropertyChanged("Delete"));
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

        public override string ToString()
        {
            var columnData = $"{this.Id + Properties.Resources.NewItemBreak + this.Title + Properties.Resources.NewItemBreak + this.ColumnVisible + Properties.Resources.NewItemBreak}";
            return this.Items.Aggregate(columnData, (current, item) => current + item + Properties.Resources.NewItemBreak);
        }
    }
}