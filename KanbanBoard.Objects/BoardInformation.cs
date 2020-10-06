using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KanbanBoard.Objects
{
    public class BoardInformation
    {
        public BoardInformation(string filePath, List<ColumnInformation> columns)
        {
            this.FilePath = filePath;
            this.Columns = new ObservableCollection<ColumnInformation>(columns);
        }

        public BoardInformation(string filePath)
        {
            this.Columns = new ObservableCollection<ColumnInformation>();
            this.ItemTypes = new ObservableCollection<string>();
            if (File.Exists(filePath))
            {
                // For each version in future releases migration will occur here! Via Assembly information check.
                var lines = File.ReadAllLines(filePath);
                this.ItemTypes = new ObservableCollection<string>(lines[1].Split(','));
                for (var i = 2; i < lines.Length; i++)
                {
                    this.Columns.Add(ColumnInformation.Load(lines[i]));
                }
            }
            else
            {
                this.Columns.Add(new ColumnInformation("New"));
                this.Columns.Add(new ColumnInformation("In Progress"));
                this.Columns.Add(new ColumnInformation("Done"));
            }

            this.FilePath = filePath;
        }

        public ObservableCollection<string> ItemTypes { get; set; }

        public string FilePath { get; }

        public ObservableCollection<ColumnInformation> Columns { get; }

        public int ColumnCount => Columns.Count;

        public void Save()
        {
            var boardData = new List<string>
            {
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), string.Join(",", this.ItemTypes)
            };

            boardData.AddRange(this.Columns.Select(columnInformation => columnInformation.ToString()));

            File.WriteAllLines(this.FilePath, boardData.ToArray());
        }

        public void RemoveColumn(ColumnInformation columnInformation)
        {
            if (!this.Columns.Remove(columnInformation))
            {
                this.Columns.Remove(this.Columns.First(x => x.ColumnId == columnInformation.ColumnId));
            }
        }

        public void AddBlankColumn(string title)
        {
            this.Columns.Add(new ColumnInformation(title));
        }

        public void InsertBlankColumn(string title)
        {
            this.Columns.Insert(0, new ColumnInformation(title));
        }

        public void MigrateItemsToLeftMost(ColumnInformation column)
        {
            var newColumnIndex = this.GetNewLeftMost(column);
            foreach (var itemInformation in column.Items)
            {
                this.Columns[newColumnIndex].Items.Add(itemInformation);
            }
        }

        public int GetColumnIndex(ColumnInformation columnInformation)
        {
            return this.Columns.IndexOf(this.Columns.First(x => x.ColumnId == columnInformation.ColumnId));
        }

        public int GetNewLeftMost(ColumnInformation columnInformation)
        {
            return this.Columns.IndexOf(this.Columns.First(x => x.ColumnId != columnInformation.ColumnId));
        }

        public int GetItemsColumnIndex(ItemInformation itemInformation)
        {
            foreach (var columnInformation in Columns)
            {
                if (columnInformation.Items.Contains(itemInformation))
                {
                    return this.GetColumnIndex(columnInformation);
                }
            }
                
            return -1;
        }
    }
}