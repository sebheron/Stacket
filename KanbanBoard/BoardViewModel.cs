using KanbanBoard.DialogWindows;
using KanbanBoard.Objects;
using KanbanBoard.Properties;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace KanbanBoard {
   public class BoardViewModel : BindableBase {

      private bool changed;
      public bool Changed {
         get => changed;
         set {
            SetProperty(ref changed, value);
         }
      }

      private BoardInformation boardInformation;
      public BoardInformation BoardInformation {
         get => boardInformation;
         set {
            SetProperty(ref boardInformation, value);
         }
      }

      private bool loadEnabled = true;
      public bool LoadEnabled {
         get => loadEnabled;
         set {
            SetProperty(ref loadEnabled, value);
         }
      }

      private bool newEnabled = true;
      public bool NewEnabled {
         get => newEnabled;
         set {
            SetProperty(ref newEnabled, value);
         }
      }

      public BoardViewModel() {
         BoardHandling.Setup();
         if (Settings.Default.CurrentBoard == null || Settings.Default.CurrentBoard == string.Empty) {
            Settings.Default.CurrentBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
            if (Settings.Default.CurrentBoard == null || Settings.Default.CurrentBoard == string.Empty) {
               Application.Current.Shutdown();
            }
            Settings.Default.Save();
         } else if (!File.Exists(Settings.Default.CurrentBoard)) {
            DialogBoxService.Show("The board " + Path.GetFileName(Settings.Default.CurrentBoard) + " is missing.", "Missing Board File");
            Settings.Default.CurrentBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
            if (Settings.Default.CurrentBoard == null || Settings.Default.CurrentBoard == string.Empty) {
               Application.Current.Shutdown();
            }
            Settings.Default.Save();
         }
         BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
      }

      //The width of the window.
      public double WindowWidth => SystemParameters.MaximizedPrimaryScreenWidth;

      //The height of the window.
      public double WindowHeight => SystemParameters.MaximizedPrimaryScreenHeight;

      //The height of an item.
      public double ItemWidth => (WindowWidth - 120) / Math.Max(5, BoardInformation.Columns.Count);

      // Show settings command.
      public ICommand ShowSettingsCommand => new DelegateCommand(ShowSettings, delegate () { return true; });

      // Board command.
      public ICommand NewBoardCommand => new DelegateCommand(NewBoard, delegate () { return true; });
      public ICommand LoadBoardCommand => new DelegateCommand(LoadBoard, delegate () { return true; });
      public ICommand SaveBoardCommand => new DelegateCommand(SaveBoard, CanSave).ObservesProperty(() => Changed);

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

      public void NewBoard() {
         if (BoardInformation.FilePath != null && BoardInformation.FilePath != string.Empty && Changed && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes")) {
            SaveBoard();
         }
         NewEnabled = false;
         LoadEnabled = false;
         string input = DialogBoxService.GetInput("Name for the new board:", "New Board");
         NewEnabled = true;
         LoadEnabled = true;
         if (input == string.Empty || input == null) {
            return;
         } else {
            Settings.Default.CurrentBoard = Path.Combine(BoardHandling.BoardFileStorageLocation, input + BoardHandling.BoardFileExtension);
            BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            Settings.Default.Save();
            Changed = false;
         }
      }

      private void LoadBoard() {
         if (Changed && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes")) {
            BoardInformation.Save();
         }
         NewEnabled = false;
         LoadEnabled = false;
         string newBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
         NewEnabled = true;
         LoadEnabled = true;
         if (newBoard == null || newBoard == string.Empty) {
            return;
         } else {
            Settings.Default.CurrentBoard = newBoard;
            BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            Settings.Default.Save();
            Changed = false;
         }
      }

      public void SaveBoard() {
         BoardInformation.Save();
         Changed = false;
      }

      public bool CanSave() {
         return Changed;
      }

      private void AddColumnLeft(object arg) {
         BoardInformation.InsertBlankColumn("New Column");
         RaisePropertyChanged(nameof(ItemWidth));
         Changed = true;
      }

      private void AddColumnRight(object arg) {
         BoardInformation.AddBlankColumn("New Column");
         RaisePropertyChanged(nameof(ItemWidth));
         Changed = true;
      }

      private void DeleteColumn(object arg) {
         if (arg is ColumnInformation columnInformation) {
            if (BoardInformation.ColumnCount <= 1) {
               DialogBoxService.Show("This is the last column and cannot be removed.", "Remove column");
               return;
            }
            if (!columnInformation.Unchanged() && !DialogBoxService.ShowYesNo("Are you sure you want to remove this column?", "Remove Column")) {
               return;
            }
            if (columnInformation.Items.Count > 0) {

               bool saveItems = DialogBoxService.ShowYesNo("Should all the items within the column be saved? If so they will be moved to the leftmost column.", "Remove Column");
               if (saveItems) {
                  BoardInformation.MigrateItemsToLeftMost(columnInformation);
               }
            }
            BoardInformation.RemoveColumn(columnInformation);
            RaisePropertyChanged(nameof(ItemWidth));
            Changed = true;
         }
      }

      private void DeleteItem(object arg) {
         if (arg is ItemInformation itemInformation) {
            bool remove = itemInformation.Unchanged() || DialogBoxService.ShowYesNo("Are you sure you want to remove this item?", "Remove Item");
            if (remove) {
               BoardInformation.Columns[BoardInformation.GetItemsColumnIndex(itemInformation)].Items.Remove(itemInformation);
               Changed = true;
            }
         }
      }

      private void AddItem(object arg) {
         if (arg is ColumnInformation columnInformation) {
            columnInformation.Items.Add(new ItemInformation("New Item"));
            Changed = true;
         }
      }

      public void OnClosing(object sender, CancelEventArgs e) {
         if (BoardInformation.FilePath != null && BoardInformation.FilePath != string.Empty && Changed && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes")) {
            BoardInformation.Save();
         }
         foreach (Window item in Application.Current.Windows) {
            if (item.Tag == null || item.Tag.ToString() != "MainWindow")
               item.Close();
         }
      }
   }
}
