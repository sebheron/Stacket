using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KanbanBoard.Logic.Properties;
using Prism.Mvvm;

namespace KanbanBoard.Logic.BoardDataTypes
{
    public class ColumnInformation : BindableBase
    {
        private const string NewItemBreak = "#&<NEWITEM>&#";

        private string columnTitle;

        private bool columnVisible;

        public ColumnInformation(string columnTitle)
        {
            this.ColumnId = Guid.NewGuid();
            this.ColumnTitle = columnTitle;
            this.ColumnVisible = false;
            this.Items = new ObservableCollection<ItemInformation>();
        }

        public ColumnInformation(Guid columnId, string columnTitle, bool columnVisible, List<ItemInformation> items)
        {
            this.ColumnId = columnId;
            this.ColumnTitle = columnTitle;
            this.ColumnVisible = columnVisible;
            this.Items = new ObservableCollection<ItemInformation>(items);
        }

        public Guid ColumnId { get; set; }

        public string ColumnTitle
        {
            get => columnTitle;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.SetProperty(ref columnTitle, value);
                }
            }
        }

        public ObservableCollection<ItemInformation> Items { get; }

        public bool ColumnVisible
        {
            get => columnVisible;
            set => SetProperty(ref columnVisible, value);
        }

        public static ColumnInformation Load(string parsedColumn)
        {
            var columnData = parsedColumn.Split(new[] { NewItemBreak }, StringSplitOptions.None);
            var containsVisiblityData = bool.TryParse(columnData[2], out bool columnVisible);
            var items = new List<ItemInformation>();
            for (var i = containsVisiblityData ? 3 : 2; i < columnData.Length - 1; i++)
            {
                items.Add(ItemInformation.Load(columnData[i]));
            }
            return new ColumnInformation(Guid.Parse(columnData[0]), columnData[1], columnVisible, items);
        }

        public override string ToString()
        {
            var columnData = $"{this.ColumnId + NewItemBreak + this.ColumnTitle + NewItemBreak + this.ColumnVisible + NewItemBreak}";
            return Items.Aggregate(columnData, (current, item) => current + item + NewItemBreak);
        }

        public bool Unchanged()
        {
            return this.ColumnTitle == Resources.Board_NewColumnName && this.Items.Count <= 0;
        }
    }
}