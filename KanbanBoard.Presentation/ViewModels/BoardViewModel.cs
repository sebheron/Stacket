using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using KanbanBoard.Logic.BoardDataTypes;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;

        private BoardInformation boardInformation;
        private bool changed;
        private bool loadEnabled = true;
        private bool newEnabled = true;

        public BoardViewModel(IDialogService dialogService, ILoggerFacade logger)
        {
            this.logger = logger;
            this.dialogService = dialogService;

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.OnLoadedCommand = new DelegateCommand(this.OnWindowLoaded);
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

        // Loaded window command.
        public ICommand OnLoadedCommand { get; }

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

        public void OnWindowLoaded()
        {
            this.logger.Log("Window successfully loaded", Category.Debug, Priority.None);
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);
        }

        private void ShowSettings()
        {
            this.dialogService.ShowSettings();
        }

        public void NewBoard()
        {
            this.logger.Log("New board requested", Category.Debug, Priority.None);
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath)
                && this.Changed
                && this.dialogService.ShowYesNo(Resources.Dialog_SaveChanges_Message, Resources.Dialog_SaveChanges_Title))
            {
                this.SaveBoard();
            }

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var input = this.dialogService.GetInput(Resources.Dialog_NewBoard_Message, Resources.Dialog_NewBoard_Title);

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(input)) return;

            Settings.Default.CurrentBoard = Path.Combine(FileLocations.BoardFileStorageLocation,
                input + Resources.BoardFileExtension);
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.logger.Log("New board created and loaded", Category.Debug, Priority.None);
            this.Changed = false;
        }

        private void LoadBoard()
        {
            this.logger.Log("Load board requested", Category.Debug, Priority.None);
            if (this.Changed && this.dialogService.ShowYesNo(Resources.Dialog_SaveChanges_Message, Resources.Dialog_SaveChanges_Title))
            {
                this.SaveBoard();
            }

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var newBoard = this.dialogService.SelectBoard();

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(newBoard)) return;

            Settings.Default.CurrentBoard = newBoard;
            this.BoardInformation = new BoardInformation(Settings.Default.CurrentBoard);
            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);
            this.Changed = false;
        }

        public void SaveBoard()
        {
            this.BoardInformation.Save();
            this.logger.Log("Board successfully saved", Category.Debug, Priority.None);
            this.Changed = false;
        }

        public bool CanSave()
        {
            return this.Changed;
        }

        private void AddColumnLeft(object arg)
        {
            this.BoardInformation.InsertBlankColumn(Resources.Board_NewColumnName);
            this.RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
            this.logger.Log("New left column created", Category.Debug, Priority.None);
        }

        private void AddColumnRight(object arg)
        {
            this.BoardInformation.AddBlankColumn(Resources.Board_NewColumnName);
            RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
            this.logger.Log("New right column created", Category.Debug, Priority.None);
            throw new InvalidCastException();
        }

        private void DeleteColumn(object arg)
        {
            if (!(arg is ColumnInformation columnInformation)) return;

            if (this.BoardInformation.ColumnCount <= 1)
            {
                this.dialogService.ShowMessage(Resources.Dialog_CannotRemoveLastColumn_Message, Resources.Dialog_RemoveColumn_Title);
                return;
            }

            if (!columnInformation.Unchanged() && !this.dialogService.ShowYesNo(Resources.Dialog_RemoveColumn_Message, Resources.Dialog_RemoveColumn_Title)) return;

            if (columnInformation.Items.Count > 0)
            {
                var saveItems = this.dialogService.ShowYesNo(Resources.Dialog_SaveItemsInColumn_Message, Resources.Dialog_RemoveColumn_Title);
                if (saveItems) BoardInformation.MigrateItemsToLeftMost(columnInformation);
            }

            this.BoardInformation.RemoveColumn(columnInformation);
            this.RaisePropertyChanged(nameof(this.ItemWidth));
            this.Changed = true;
            this.logger.Log("Column deleted", Category.Debug, Priority.None);
        }

        private void DeleteItem(object arg)
        {
            if (arg is ItemInformation itemInformation)
            {
                var remove = itemInformation.Unchanged() ||
                             this.dialogService.ShowYesNo(Resources.Dialog_RemoveItem_Message, Resources.Dialog_RemoveItem_Title);
                if (remove)
                {
                    this.BoardInformation.Columns[BoardInformation.GetItemsColumnIndex(itemInformation)].Items
                        .Remove(itemInformation);
                    this.Changed = true;
                }
            }
            this.logger.Log("Item deleted", Category.Debug, Priority.None);
        }

        private void AddItem(object arg)
        {
            if (arg is ColumnInformation columnInformation)
            {
                columnInformation.Items.Add(new ItemInformation(Resources.Board_NewItemName));
                this.Changed = true;
            }
            this.logger.Log("Item created", Category.Debug, Priority.None);
        }

        private void OnClosing()
        {
            this.logger.Log("Stacket closing", Category.Debug, Priority.None);
            if (!string.IsNullOrEmpty(this.BoardInformation.FilePath)
                && this.dialogService.ShowYesNo(Resources.Dialog_SaveChanges_Message, Resources.Dialog_SaveChanges_Title))
            {
                this.SaveBoard();
            }
            Application.Current.Shutdown();
        }
    }
}