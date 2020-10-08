using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using KanbanBoard.Behaviors;
using KanbanBoard.DialogWindows;
using KanbanBoard.Objects;
using KanbanBoard.Properties;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard
{
    public class BoardViewModel : BindableBase
    {
        private BoardInformation boardInformation;
        private bool changed;
        private bool loadEnabled = true;
        private bool newEnabled = true;

        public BoardViewModel()
        {
            if (!Settings.Default.AskedUserForStartup) {
                if (DialogBoxService.ShowYesNo("Should Stacket start on Windows startup?", "Stacket")) {
                    Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)
                        .SetValue("Stacket", Process.GetCurrentProcess().MainModule.FileName);
                }
                Settings.Default.AskedUserForStartup = true;
            }
            BoardHandling.Setup();
            if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
            {
                Settings.Default.CurrentBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
                    
                Settings.Default.Save();
            }
            else if (!File.Exists(Settings.Default.CurrentBoard))
            {
                DialogBoxService.Show("The board " + Path.GetFileName(Settings.Default.CurrentBoard) + " is missing.", "Missing Board File");
                Settings.Default.CurrentBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
                    
                Settings.Default.Save();
            }

            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.ShowSettingsCommand = new DelegateCommand(ShowSettings, () => true);
            this.NewBoardCommand = new DelegateCommand(NewBoard, () => true);
            this.LoadBoardCommand = new DelegateCommand(LoadBoard, () => true);
            this.SaveBoardCommand = new DelegateCommand(SaveBoard, CanSave).ObservesProperty(() => Changed);
            this.AddColumnLeftCommand = new DelegateCommand<object>(AddColumnLeft, arg => true);
            this.AddColumnRightCommand = new DelegateCommand<object>(AddColumnRight, arg => true);
            this.DeleteColumnCommand = new DelegateCommand<object>(DeleteColumn, arg => true);
            this.AddItemCommand = new DelegateCommand<object>(AddItem, arg => true);
            this.DeleteItemCommand = new DelegateCommand<object>(DeleteItem, arg => true);
        }

        public DragHandleBehavior DragHandler { get; }

        public bool Changed
        {
            get => this.changed;
            set => SetProperty(ref this.changed, value);
        }

        public BoardInformation BoardInformation
        {
            get => this.boardInformation;
            set => SetProperty(ref this.boardInformation, value);
        }

        public bool LoadEnabled
        {
            get => this.loadEnabled;
            set => SetProperty(ref this.loadEnabled, value);
        }

        public bool NewEnabled
        {
            get => this.newEnabled;
            set => SetProperty(ref this.newEnabled, value);
        }

        //The width of the window.
        public double WindowWidth => SystemParameters.MaximizedPrimaryScreenWidth;

        //The height of the window.
        public double WindowHeight => SystemParameters.MaximizedPrimaryScreenHeight;

        //The height of an item.
        public double ItemWidth => (WindowWidth - 120) / Math.Max(5, BoardInformation.Columns.Count);

        // Show settings command.
        public ICommand ShowSettingsCommand { get; }

        // Board command.
        public ICommand NewBoardCommand { get; }
        public ICommand LoadBoardCommand { get; }
        public ICommand SaveBoardCommand { get; }

        // Column commands.
        public ICommand AddColumnLeftCommand { get; }
        public ICommand AddColumnRightCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        // Item commands.
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }

        private void ShowSettings()
        {
            //Show settings
        }

        public void NewBoard()
        {
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath) 
                && this.Changed 
                && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                this.SaveBoard();
            }
                
            this.NewEnabled = false;
            this.LoadEnabled = false;

            var input = DialogBoxService.GetInput("Name for the new board:", "New Board");
            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(input)) return;

            Settings.Default.CurrentBoard = Path.Combine(BoardHandling.BoardFileStorageLocation,
                input + BoardHandling.BoardFileExtension);
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            Settings.Default.Save();
            this.Changed = false;
        }

        private void LoadBoard()
        {
            if (this.Changed && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                this.BoardInformation.Save();
            }


            this.NewEnabled = false;
            this.LoadEnabled = false;
            var newBoard = DialogBoxService.SelectBoard(Settings.Default.CurrentBoard);
            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(newBoard)) return;

            Settings.Default.CurrentBoard = newBoard;
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            Settings.Default.Save();
            this.Changed = false;
        }

        public void SaveBoard()
        {
            this.BoardInformation.Save();
            this.Changed = false;
        }

        public bool CanSave()
        {
            return this.Changed;
        }

        private void AddColumnLeft(object arg)
        {
            this.BoardInformation.InsertBlankColumn("New Column");
            this.RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
        }

        private void AddColumnRight(object arg)
        {
            this.BoardInformation.AddBlankColumn("New Column");
            RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
        }

        private void DeleteColumn(object arg)
        {
            if (!(arg is ColumnInformation columnInformation)) return;

            if (this.BoardInformation.ColumnCount <= 1)
            {
                DialogBoxService.Show("This is the last column and cannot be removed.", "Remove column");
                return;
            }

            if (!columnInformation.Unchanged() && !DialogBoxService.ShowYesNo("Are you sure you want to remove this column?", "Remove Column")) return;

            if (columnInformation.Items.Count > 0)
            {
                var saveItems = DialogBoxService.ShowYesNo(
                    "Should all the items within the column be saved? If so they will be moved to the leftmost column.",
                    "Remove Column");
                if (saveItems) BoardInformation.MigrateItemsToLeftMost(columnInformation);
            }

            this.BoardInformation.RemoveColumn(columnInformation);
            this.RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
        }

        private void DeleteItem(object arg)
        {
            if (arg is ItemInformation itemInformation)
            {
                var remove = itemInformation.Unchanged() ||
                             DialogBoxService.ShowYesNo("Are you sure you want to remove this item?", "Remove Item");
                if (remove)
                {
                    this.BoardInformation.Columns[BoardInformation.GetItemsColumnIndex(itemInformation)].Items
                        .Remove(itemInformation);
                    this.Changed = true;
                }
            }
        }

        private void AddItem(object arg)
        {
            if (arg is ColumnInformation columnInformation)
            {
                columnInformation.Items.Add(new ItemInformation("New Item"));
                this.Changed = true;
            }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath)
                && this.Changed
                && DialogBoxService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                BoardInformation.Save();
            }

            foreach (Window item in Application.Current.Windows)
            {
                if (item.Tag == null || item.Tag.ToString() != "MainWindow")
                {
                    item.Close();
                }
            }
        }
    }
}