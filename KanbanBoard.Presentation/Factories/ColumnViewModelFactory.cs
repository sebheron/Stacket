using System;
using System.Collections.Generic;
using KanbanBoard.Presentation.Services;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;
using Prism.Logging;

namespace KanbanBoard.Presentation.Factories
{
    public class ColumnViewModelFactory : IColumnViewModelFactory
    {
        private readonly IItemViewModelFactory itemFactory;
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;
        private readonly IEventAggregator eventAggregator;

        public ColumnViewModelFactory(
            IItemViewModelFactory itemFactory, 
            IDialogService dialogService, 
            ILoggerFacade logger, 
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.itemFactory = itemFactory;
            this.dialogService = dialogService;
            this.logger = logger;
        }

        public ColumnViewModel CreateColumn(string title = null)
        {
            var column = new ColumnViewModel(this.itemFactory, this.dialogService, this.logger, this.eventAggregator);

            if (!string.IsNullOrEmpty(title))
            {
                column.Title = title;
            }

            return column;
        }

        public ColumnViewModel Load(string columnData)
        {
            var parsedColumn = columnData.Split(new[] { Properties.Resources.NewItemBreak }, StringSplitOptions.None);
            var containsVisibilityData = bool.TryParse(parsedColumn[2], out var columnVisible);

            var items = new List<ItemViewModel>();
            for (var i = containsVisibilityData ? 3 : 2; i < parsedColumn.Length - 1; i++)
            {
                items.Add(this.itemFactory.Load(parsedColumn[i]));
            }

            return new ColumnViewModel(
                Guid.Parse(parsedColumn[0]), 
                parsedColumn[1], 
                columnVisible, 
                items, 
                this.itemFactory, 
                this.dialogService, 
                this.logger, 
                this.eventAggregator);
        }
    }
}