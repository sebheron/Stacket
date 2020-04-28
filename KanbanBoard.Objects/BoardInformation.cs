using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KanbanBoard.Objects {
   public class BoardInformation {
      public string FilePath { get; }

      public ObservableCollection<ColumnInformation> Columns { get; }

      public int ColumnCount => Columns.Count;

      public List<string> ColumnTitles => Columns.Select(x => x.ColumnTitle).ToList();

      public BoardInformation(string filePath, List<ColumnInformation> columns) {
         FilePath = filePath;
         Columns = new ObservableCollection<ColumnInformation>(columns);
      }

      public BoardInformation(string filePath) {
         Columns = new ObservableCollection<ColumnInformation>();
         if (File.Exists(filePath)) {
            // For each version in future releases migration will occur here!
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) {
               Columns.Add(ColumnInformation.Load(lines[i]));
            }
         }
         FilePath = filePath;
      }

      public void Save(string filePath) {
         List<string> boardData = new List<string>();
         boardData.Add(Assembly.GetExecutingAssembly().GetName().Version.ToString());
         foreach (ColumnInformation columnInformation in Columns) {
            boardData.Add(columnInformation.ToString());
         }
         File.WriteAllLines(filePath, boardData.ToArray());
      }

      public void Save() {
         List<string> boardData = new List<string>();
         boardData.Add(Assembly.GetExecutingAssembly().GetName().Version.ToString());
         foreach (ColumnInformation columnInformation in Columns) {
            boardData.Add(columnInformation.ToString());
         }
         File.WriteAllLines(FilePath, boardData.ToArray());
      }

      public void RemoveColumn(ColumnInformation columnInformation) {
         if (Columns.Remove(columnInformation) == false)
            Columns.Remove(Columns.First(x => x.ColumnID == columnInformation.ColumnID));
      }

      public void AddBlankColumn(string title) {
         Columns.Add(new ColumnInformation(title));
      }

      public void InsertBlankColumn(string title) {
         Columns.Insert(0, new ColumnInformation(title));
      }

      public void MigrateItemsToLeftMost(ColumnInformation column) {
         int newColumnIndex = GetNewLeftMost(column);
         foreach (ItemInformation itemInformation in column.Items) {
            Columns[newColumnIndex].Items.Add(itemInformation);
         }
      }

      public int GetColumnIndex(ColumnInformation columnInformation) {
         return Columns.IndexOf(Columns.First(x => x.ColumnID == columnInformation.ColumnID));
      }

      public int GetNewLeftMost(ColumnInformation columnInformation) {
         return Columns.IndexOf(Columns.First(x => x.ColumnID != columnInformation.ColumnID));
      }

      public int GetItemsColumnIndex(ItemInformation itemInformation) {
         foreach (ColumnInformation columnInformation in Columns) {
            if (columnInformation.Items.Contains(itemInformation)) {
               return GetColumnIndex(columnInformation);
            }
         }
         return -1;
      }
   }
}
