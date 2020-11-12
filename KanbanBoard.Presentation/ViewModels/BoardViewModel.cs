using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Kanban.Core.Events;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Factories;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly IColumnViewModelFactory columnFactory;
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;
        private readonly IEventAggregator eventAggregator;

        private string filePath => Settings.Default.CurrentBoard;
        private bool changed;
        private bool loadEnabled = true;
        private bool newEnabled = true;
        private bool loadInProgress;

        public BoardViewModel(
            IColumnViewModelFactory columnFactory, 
            IDialogService dialogService, 
            ILoggerFacade logger, 
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteColumn);
            this.eventAggregator.GetEvent<RequestSaveEvent>().Subscribe(this.SaveBoard);

            this.columnFactory = columnFactory;
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

            this.AddColumnLeftCommand = new DelegateCommand(this.AddColumnLeft);
            this.AddColumnRightCommand = new DelegateCommand(this.AddColumnRight);
            /*this.DeleteColumnCommand = new DelegateCommand<object>(this.DeleteColumn);
            this.AddItemCommand = new DelegateCommand<object>(this.AddItem);
            this.DeleteItemCommand = new DelegateCommand<object>(this.DeleteItem);*/
        }

        public ObservableCollection<ColumnViewModel> Columns { get; } = new ObservableCollection<ColumnViewModel>();

        public DragHandleBehavior DragHandler { get; }

        public bool Changed
        {
            get => this.changed;
            set => SetProperty(ref this.changed, value);
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

        public double WindowWidth => SystemParameters.MaximizedPrimaryScreenWidth;
        public double WindowHeight => SystemParameters.MaximizedPrimaryScreenHeight;
        public double ColumnWidth => (this.WindowWidth - 120) / Math.Max(5, this.Columns.Count);

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
        //public ICommand DeleteColumnCommand { get; }

        /*public ICommand AddItemCommand { get; }

        public ICommand DeleteItemCommand { get; }*/

        private void OnWindowLoaded()
        {
            this.logger.Log("Window successfully loaded", Category.Debug, Priority.None);
            this.LoadBoardFromFile();
            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);

            this.Columns.CollectionChanged += this.ColumnsChanged;
        }

        private void ShowSettings()
        {
            this.dialogService.ShowSettings();
        }

        private void NewBoard()
        {
            this.logger.Log("New board requested", Category.Debug, Priority.None);
            if (!string.IsNullOrEmpty(this.filePath)
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

            Settings.Default.CurrentBoard = Path.Combine(FileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);
            this.LoadBoardFromFile();

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
            this.LoadBoardFromFile();
            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);
            this.Changed = false;
        }

        private void LoadBoardFromFile()
        {
            this.loadInProgress = true;
            this.Columns.Clear();

            if (File.Exists(this.filePath))
            {
                // For each version in future releases migration will occur here! Via Assembly information check.
                var lines = File.ReadAllLines(this.filePath);
                
                for (var i = 2; i < lines.Length; i++)
                {
                    this.Columns.Add(this.columnFactory.Load(lines[i]));
                }
            }
            else
            {
                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_New));
                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_InProgress));
                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_Done));
            }

            this.loadInProgress = false;
        }

        public void SaveBoard()
        {
            if (this.loadInProgress || string.IsNullOrEmpty(this.filePath)) return;

            var boardData = new List<string> { Assembly.GetExecutingAssembly().GetName().Version.ToString(), string.Empty };

            boardData.AddRange(this.Columns.Select(column => column.ToString()));

            File.WriteAllLines(this.filePath, boardData.ToArray());

            this.logger.Log("Board successfully saved", Category.Debug, Priority.None);
        }

        public bool CanSave()
        {
            return this.Changed;
        }

        private void AddColumnLeft()
        {
            this.Columns.Insert(0, this.columnFactory.CreateColumn());
            this.RaisePropertyChanged(nameof(this.ColumnWidth));
            
            this.logger.Log("New left column created", Category.Debug, Priority.None);
        }

        private void AddColumnRight()
        {
            this.Columns.Add(this.columnFactory.CreateColumn());
            RaisePropertyChanged(nameof(this.ColumnWidth));
            this.Changed = true;
            this.logger.Log("New right column created", Category.Debug, Priority.None);
        }

        private void DeleteColumn(Guid columnId)
        {
            if (this.Columns.Count <= 1)
            {
                this.dialogService.ShowMessage(Resources.Dialog_CannotRemoveLastColumn_Message, Resources.Dialog_RemoveColumn_Title);
                return;
            }

            var columnToDelete = this.Columns.FirstOrDefault(column => column.Id == columnId);
            if (columnToDelete == null) return;

            if (!columnToDelete.Unchanged && !this.dialogService.ShowYesNo(Resources.Dialog_RemoveColumn_Message, Resources.Dialog_RemoveColumn_Title)) return;

            var items = columnToDelete.Items;

            this.Columns.Remove(columnToDelete);
            this.logger.Log("Column deleted", Category.Debug, Priority.None);

            if (items.Count <= 0 || !this.dialogService.ShowYesNo(Resources.Dialog_SaveItemsInColumn_Message, Resources.Dialog_RemoveColumn_Title)) return;

            foreach (var item in items)
            {
                this.Columns[0].Items.Add(item);
            }

            this.logger.Log("Items migrated to left-most column", Category.Debug, Priority.None);
        }

        /*private void DeleteItem(object arg)
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
        }*/

        /*private void AddItem(object arg)
        {
            if (arg is ColumnInformation columnInformation)
            {
                columnInformation.Items.Add(new ItemInformation(Resources.Board_NewItemName));
                this.Changed = true;
            }
            this.logger.Log("Item created", Category.Debug, Priority.None);
        }*/

        private void ColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.ColumnWidth));
            this.Changed = true;

            this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        private void OnClosing()
        {
            this.logger.Log("Stacket closing", Category.Debug, Priority.None);
            if (!string.IsNullOrEmpty(this.filePath)
                && this.dialogService.ShowYesNo(Resources.Dialog_SaveChanges_Message, Resources.Dialog_SaveChanges_Title))
            {
                this.SaveBoard();
            }

            Application.Current.Shutdown();
        }
    }
}