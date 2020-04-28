using KanbanBoard.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KanbanBoard {
   public static class BoardHandling {

      public static string BoardFileStorageLocation => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Boards");

      public static void Setup() {
         Directory.CreateDirectory(BoardFileStorageLocation);
      }
   }

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
            string[] lines = File.ReadAllLines(filePath);
            foreach (string parsedColumn in lines) {
               Columns.Add(ColumnInformation.Load(parsedColumn));
            }
         }
         FilePath = filePath;
      }

      public void Save(string filePath) {
         List<string> boardData = new List<string>();
         foreach (ColumnInformation columnInformation in Columns) {
            boardData.Add(columnInformation.ToString());
         }
         File.WriteAllLines(filePath, boardData.ToArray());
      }

      public void Save() {
         List<string> boardData = new List<string>();
         foreach (ColumnInformation columnInformation in Columns) {
            boardData.Add(columnInformation.ToString());
         }
         File.WriteAllLines(FilePath, boardData.ToArray());
      }

      public bool HasTitle(string title) {
         return Columns.Any(x => x.ColumnTitle == title);
      }

      public void RemoveColumn(ColumnInformation columnInformation) {
         if (Columns.Remove(columnInformation) == false)
            Columns.Remove(Columns.First(x => x.ColumnTitle == columnInformation.ColumnTitle));
      }

      public void AddBlankColumn(string title) {
         Columns.Add(new ColumnInformation(title));
      }

      public void InsertBlankColumn(string title) {
         Columns.Insert(0, new ColumnInformation(title));
      }

      public void MigrateItemsToLeftMost(ColumnInformation column) {
         int newColumnIndex = GetNewLeftMost(column.ColumnTitle);
         foreach (ItemInformation itemInformation in column.Items) {
            Columns[newColumnIndex].Items.Add(itemInformation);
         }
      }

      public int GetColumnIndex(string title) {
         return Columns.IndexOf(Columns.First(x => x.ColumnTitle == title));
      }

      public int GetNewLeftMost(string title) {
         return Columns.IndexOf(Columns.First(x => x.ColumnTitle != title));
      }

      public ColumnInformation GetColumn(int index) {
         return Columns[index];
      }

      public int GetItemsColumnIndex(ItemInformation itemInformation) {
         foreach (ColumnInformation columnInformation in Columns) {
            if (columnInformation.Items.Contains(itemInformation)) {
               return GetColumnIndex(columnInformation.ColumnTitle);
            }
         }
         return -1;
      }
   }

   public class ColumnInformation {
      public ColumnInformation This => this;

      public string ColumnTitle { get; set; }

      public bool Visible { get; set; }

      public ObservableCollection<ItemInformation> Items { get; }

      public ColumnInformation(string columnTitle) {
         ColumnTitle = columnTitle;
         Items = new ObservableCollection<ItemInformation>();
         Visible = true;
      }

      public ColumnInformation(string columnTitle, List<ItemInformation> items) {
         ColumnTitle = columnTitle;
         Items = new ObservableCollection<ItemInformation>(items);
      }

      public static ColumnInformation Load(string parsedColumn) {
         string[] columnData = parsedColumn.Split(new string[] { "#&<NEWITEM>&#" }, StringSplitOptions.None);
         List <ItemInformation> items = new List<ItemInformation>();
         for (int i = 1; i < columnData.Length - 1; i++) {
            items.Add(ItemInformation.Load(columnData[i]));
         }
         return new ColumnInformation(columnData[0], items);
      }

      public override string ToString() {
         StringBuilder columnData = new StringBuilder();
         columnData.Append(ColumnTitle + "#&<NEWITEM>&#");
         foreach (ItemInformation item in Items) {
            columnData.Append(item.ToString() + "#&<NEWITEM>&#");
         }
         return columnData.ToString();
      }
   }

   public class ItemInformation {
      public string ItemTitle { get; set; }
      public string ItemDescription { get; set; }
      public ItemTypes ItemType { get; set; }
      public DateTime ItemDueDate { get; set; }
      public Color ItemColor { get; set; }

      public ItemInformation(string itemTitle) {
         ItemTitle = itemTitle;
         ItemDescription = string.Empty;
         ItemType = ItemTypes.Bug;
         ItemDueDate = DateTime.Now.Date;
         ItemColor = Color.FromArgb(255,255,0,0);
      }

      public ItemInformation(string itemTitle, string itemDescription, ItemTypes itemType, DateTime itemDueDate, Color itemColor) {
         ItemTitle = itemTitle;
         ItemDescription = itemDescription;
         ItemType = itemType;
         ItemDueDate = itemDueDate.Date;
         ItemColor = itemColor;
      }

      public static ItemInformation Load(string parsedItem) {
         string[] itemData = parsedItem.Split(new string[] { "$%<NEWITEMDATA>%$" }, StringSplitOptions.None);
         string itemTitle = itemData[0];
         string itemDescription = itemData[1];
         ItemTypes itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), itemData[2]);
         DateTime itemDueDate = DateTime.Parse(itemData[3]).Date;
         Color itemColor = (Color)ColorConverter.ConvertFromString(itemData[4]);

         return new ItemInformation(itemTitle, itemDescription, itemType, itemDueDate, itemColor);
      }

      public override string ToString() {
         StringBuilder itemData = new StringBuilder();
         itemData.Append(ItemTitle);
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemDescription);
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemType.ToString());
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemDueDate.ToString());
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemColor.ToString());
         return itemData.ToString();
      }
   }

   public enum ItemTypes {
      Item,
      ProofOfConcept,
      Bug,
      Investigation
   }
}