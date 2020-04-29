using KanbanBoard.DialogWindows;
using KanbanBoard.Objects;
using KanbanBoard.Properties;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace KanbanBoard {
   public class BoardViewModel : BindableBase {

      private bool enabled;

      public BoardInformation BoardInformation { get; set; }

      public BoardViewModel() {
         enabled = true;
         BoardHandling.Setup();
         BoardInformation = new BoardInformation(@"C:\Users\Sebhe\source\repos\KanbanBoard\KanbanBoard\bin\Debug\Boards\test.txt");
      }

      //The visibility of the window.
      public Visibility Visible => Enabled ? Visibility.Visible : Visibility.Hidden;

      //Enabled variable for whether the board should be able to be seen.
      public bool Enabled {
         get => enabled;

         set => SetProperty(ref enabled, value, () => RaisePropertyChanged(nameof(Visible)));
      }

      //The width of the window.
      public double WindowWidth => SystemParameters.MaximizedPrimaryScreenWidth;

      //The height of the window.
      public double WindowHeight => SystemParameters.MaximizedPrimaryScreenHeight;

      //The height of an item.
      public double ItemWidth => (WindowWidth - 120) / Math.Max(5, BoardInformation.Columns.Count);

      //The top position of the window.
      public double WindowTop {
         get => Settings.Default.WindowTop;
         set {
            Settings.Default.WindowTop = (int)value;
         }
      }

      //The left position of the window.
      public double WindowLeft {
         get => Settings.Default.WindowLeft;
         set {
            Settings.Default.WindowLeft = (int)value;
         }
      }

      // Show settings command.
      public ICommand ShowSettingsCommand => new DelegateCommand(ShowSettings, delegate () { return true; } );

      // Column commands.
      public ICommand AddColumnLeftCommand => new DelegateCommand<object>(AddColumnLeft, delegate (object arg) { return true; });
      public ICommand AddColumnRightCommand => new DelegateCommand<object>(AddColumnRight, delegate (object arg) { return true; });
      public ICommand DeleteColumnCommand => new DelegateCommand<object>(DeleteColumn, delegate (object arg) { return true; } );

      // Item commands.
      public ICommand AddItemCommand => new DelegateCommand<object>(AddItem, delegate (object arg) { return true; });
      public ICommand DeleteItemCommand => new DelegateCommand<object>(DeleteItem, delegate (object arg) { return true; });

      private void ShowSettings() {
         //Show settings
      }

      private void AddColumnLeft(object arg) {
         BoardInformation.InsertBlankColumn("New Column");
         RaisePropertyChanged(nameof(ItemWidth));
      }

      private void AddColumnRight(object arg) {
         BoardInformation.AddBlankColumn("New Column");
         RaisePropertyChanged(nameof(ItemWidth));
      }

      private void DeleteColumn(object arg) {
         if (arg is ColumnInformation columnInformation) {
            if (BoardInformation.ColumnCount <= 1 || !DialogBoxService.Show("Are you sure you want to remove this column?", "Remove Column").Value) {
               return;
            }
            if (columnInformation.Items.Count > 0) {

               bool? saveItems = DialogBoxService.Show("Should all the items within the column be saved? If so they will be moved to the leftmost column.", "Remove Column");
               if (saveItems.Value) {
                  BoardInformation.MigrateItemsToLeftMost(columnInformation);
               }
            }
            BoardInformation.RemoveColumn(columnInformation);
            RaisePropertyChanged(nameof(ItemWidth));
         }
      }

      private void DeleteItem(object arg) {
         if (arg is ItemInformation itemInformation) {
            bool remove = itemInformation.Unchanged() || DialogBoxService.Show("Are you sure you want to remove this item?", "Remove Item").Value;
            if (remove)
               BoardInformation.Columns[BoardInformation.GetItemsColumnIndex(itemInformation)].Items.Remove(itemInformation);
         }
      }

      private void AddItem(object arg) {
         if (arg is ColumnInformation columnInformation)
            columnInformation.Items.Add(new ItemInformation("New Item"));
      }

      public void OnClosing(object sender, CancelEventArgs e) {
         BoardInformation.Save();
      }
   }
}
