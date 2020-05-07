using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KanbanBoard.Objects
{
    public class ColumnInformation : BindableBase
    {
        public Guid ColumnID { get; set; }

        public ColumnInformation This => this;

        public string ColumnTitle { get; set; }

        public ObservableCollection<ItemInformation> Items { get; }

        private bool columnVisible = true;
        public bool ColumnVisible {
            get => columnVisible;
            set {
                SetProperty(ref columnVisible, value);
            }
        }

        public ColumnInformation(string columnTitle) {
            ColumnID = Guid.NewGuid();
            ColumnTitle = columnTitle;
            Items = new ObservableCollection<ItemInformation>();
        }

        public ColumnInformation(Guid columnId, string columnTitle, List<ItemInformation> items) {
            ColumnID = columnId;
            ColumnTitle = columnTitle;
            Items = new ObservableCollection<ItemInformation>(items);
        }

        public static ColumnInformation Load(string parsedColumn) {
            string[] columnData = parsedColumn.Split(new string[] { "#&<NEWITEM>&#" }, StringSplitOptions.None);
            List<ItemInformation> items = new List<ItemInformation>();
            for (int i = 2; i < columnData.Length - 1; i++) {
                items.Add(ItemInformation.Load(columnData[i]));
            }
            return new ColumnInformation(Guid.Parse(columnData[0]), columnData[1], items);
        }

        public override string ToString() {
            StringBuilder columnData = new StringBuilder();
            columnData.Append(ColumnID + "#&<NEWITEM>&#");
            columnData.Append(ColumnTitle + "#&<NEWITEM>&#");
            foreach (ItemInformation item in Items) {
                columnData.Append(item.ToString() + "#&<NEWITEM>&#");
            }
            return columnData.ToString();
        }

        public bool Unchanged() {
            return ColumnTitle == "New Column" && Items.Count <= 0;
        }
    }
}
