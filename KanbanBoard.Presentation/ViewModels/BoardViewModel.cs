using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using KanbanBoard.Logic.BoardDataTypes;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Presentation.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly IDialogService dialogService;

        private BoardInformation boardInformation;
        private bool changed;
        private bool loadEnabled = true;
        private bool newEnabled = true;

        public BoardViewModel(IDialogService dialogService, IRegistryService registryService, IStartupService startupService)
        {
            Settings.Default.PropertyChanged += SaveSettings;
            startupService.Initialize();

            this.dialogService = dialogService;

            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.ShowSettingsCommand = new DelegateCommand(this.ShowSettings);
            this.NewBoardCommand = new DelegateCommand(this.NewBoard);
            this.LoadBoardCommand = new DelegateCommand(this.LoadBoard);
            this.SaveBoardCommand = new DelegateCommand(this.SaveBoard, this.CanSave).ObservesProperty(() => this.Changed);
            this.ExitCommand = new DelegateCommand(this.OnClosing);

            this.AddColumnLeftCommand = new DelegateCommand<object>(this.AddColumnLeft);
            this.AddColumnRightCommand = new DelegateCommand<object>(this.AddColumnRight);
            this.DeleteColumnCommand = new DelegateCommand<object>(this.DeleteColumn);
            this.AddItemCommand = new DelegateCommand<object>(this.AddItem);
            this.DeleteItemCommand = new DelegateCommand<object>(this.DeleteItem);
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

        // Board commands.
        public ICommand NewBoardCommand { get; }

        public ICommand LoadBoardCommand { get; }
        public ICommand SaveBoardCommand { get; }

        // Exit command
        public ICommand ExitCommand { get; }

        // Column commands.
        public ICommand AddColumnLeftCommand { get; }

        public ICommand AddColumnRightCommand { get; }
        public ICommand DeleteColumnCommand { get; }

        // Item commands.
        public ICommand AddItemCommand { get; }

        public ICommand DeleteItemCommand { get; }

        private void ShowSettings()
        {
            this.dialogService.ShowSettings();
        }

        public void NewBoard()
        {
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath)
                && this.Changed
                && this.dialogService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                this.SaveBoard();
            }

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var input = this.dialogService.GetInput("Name for the new board:", "New Board");

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(input)) return;

            Settings.Default.CurrentBoard = Path.Combine(FileLocations.BoardFileStorageLocation,
                input + Resources.BoardFileExtension);
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.Changed = false;
        }

        private void LoadBoard()
        {
            if (this.Changed && this.dialogService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                this.BoardInformation.Save();
            }

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var newBoard = this.dialogService.SelectBoard();

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(newBoard)) return;

            Settings.Default.CurrentBoard = newBoard;
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
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
                this.dialogService.ShowMessage("This is the last column and cannot be removed.", "Remove column");
                return;
            }

            if (!columnInformation.Unchanged() && !this.dialogService.ShowYesNo("Are you sure you want to remove this column?", "Remove Column")) return;

            if (columnInformation.Items.Count > 0)
            {
                var saveItems = this.dialogService.ShowYesNo(
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
                             this.dialogService.ShowYesNo("Are you sure you want to remove this item?", "Remove Item");
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

        private void OnClosing()
        {
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath)
                && this.dialogService.ShowYesNo("Do you want to save changes to the current board?", "Save Changes"))
            {
                this.BoardInformation.Save();
            }

            Application.Current.Shutdown();
        }

        private void SaveSettings(object sender, PropertyChangedEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}