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
using System.ComponentModel;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly IColumnViewModelFactory columnFactory;
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;

        private string filePath => Settings.Default.CurrentBoard;
        private bool loadEnabled = true;
        private bool newEnabled = true;
        private bool loadInProgress;

        public BoardViewModel(
            IColumnViewModelFactory columnFactory,
            IDialogService dialogService,
            ILoggerFacade logger)
        {
            this.columnFactory = columnFactory;
            this.logger = logger;
            this.dialogService = dialogService;

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.OnLoadedCommand = new DelegateCommand(this.OnWindowLoaded);
            this.ShowSettingsCommand = new DelegateCommand(this.ShowSettings);
            this.NewBoardCommand = new DelegateCommand(this.NewBoard);
            this.LoadBoardCommand = new DelegateCommand(this.LoadBoard);
            this.ExitCommand = new DelegateCommand(this.OnClosing);

            this.AddColumnLeftCommand = new DelegateCommand(this.AddColumnLeft);
            this.AddColumnRightCommand = new DelegateCommand(this.AddColumnRight);
        }

        public ObservableCollection<ColumnViewModel> Columns { get; } = new ObservableCollection<ColumnViewModel>();

        public DragHandleBehavior DragHandler { get; }

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

        // Exit command
        public ICommand ExitCommand { get; }

        // Column commands.
        public ICommand AddColumnLeftCommand { get; }

        public ICommand AddColumnRightCommand { get; }

        private void OnWindowLoaded()
        {
            this.logger.Log("Window successfully loaded", Category.Debug, Priority.None);
            this.LoadBoardFromFile();
            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);

            this.Columns.CollectionChanged += (o, e) => this.RaisePropertyChanged(nameof(this.ColumnWidth));
        }

        private void ShowSettings()
        {
            this.dialogService.ShowSettings();
        }

        private void NewBoard()
        {
            this.logger.Log("New board requested", Category.Debug, Priority.None);

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var input = this.dialogService.GetInput(Resources.Dialog_NewBoard_Message, Resources.Dialog_NewBoard_Title);

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(input)) return;

            Settings.Default.CurrentBoard = Path.Combine(FileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);
            this.LoadBoardFromFile();

            this.logger.Log("New board created and loaded", Category.Debug, Priority.None);
        }

        private void LoadBoard()
        {
            this.logger.Log("Load board requested", Category.Debug, Priority.None);

            this.NewEnabled = false;
            this.LoadEnabled = false;

            var newBoard = this.dialogService.SelectBoard();

            this.NewEnabled = true;
            this.LoadEnabled = true;

            if (string.IsNullOrEmpty(newBoard)) return;

            Settings.Default.CurrentBoard = newBoard;
            this.LoadBoardFromFile();

            this.logger.Log("Board successfully loaded", Category.Debug, Priority.None);
        }

        private void LoadBoardFromFile()
        {
            this.loadInProgress = true;
            this.Columns.Clear();
            this.Columns.CollectionChanged += this.BoardPropertiesChanged;

            if (File.Exists(this.filePath))
            {
                this.logger.Log("File exists, loading from file", Category.Debug, Priority.None);

                // For each version in future releases migration will occur here! Via Assembly information check.
                var lines = File.ReadAllLines(this.filePath);

                for (var i = 2; i < lines.Length; i++)
                {
                    var column = this.columnFactory.Load(lines[i]);
                    this.Columns.Add(column);
                }

                this.loadInProgress = false;
            }
            else
            {
                this.logger.Log("File doesn't exist, creating default board", Category.Debug, Priority.None);

                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_New));
                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_InProgress));
                this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_Done));

                this.loadInProgress = false;

                // Create the board file with default data.
                this.SaveBoard();
            }
        }

        public void SaveBoard()
        {
            if (this.loadInProgress || string.IsNullOrEmpty(this.filePath)) return;

            var boardData = new List<string> { Assembly.GetExecutingAssembly().GetName().Version.ToString(), string.Empty };

            boardData.AddRange(this.Columns.Select(column => column.ToString()));

            File.WriteAllLines(this.filePath, boardData.ToArray());

            this.logger.Log("Board successfully saved", Category.Debug, Priority.None);
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

            this.logger.Log("New right column created", Category.Debug, Priority.None);
        }

        private void OpenedOptions(BaseCollectionItemViewModel vm)
        {
            var vals = this.Columns.SelectMany(column => column.Items.Where(item => item.Id != vm.Id && item.OptionsOpen).Select(item => (BaseCollectionItemViewModel)item)).ToList();
            vals.AddRange(this.Columns.Where(column => column.Id != vm.Id && column.OptionsOpen).Select(column => (BaseCollectionItemViewModel)column).ToList());

            foreach (var val in vals)
            {
                val.OptionsOpen = false;
            }
        }

        private void DeleteBaseCollection(BaseCollectionItemViewModel vm)
        {
            if (vm is ItemViewModel ivm)
            {
                var column = this.Columns.Where(c => c.Items.Contains(ivm)).FirstOrDefault();
                if (column != null)
                {
                    column.Items.Remove(ivm);
                }
                this.logger.Log("Item deleted", Category.Debug, Priority.None);
            }
            else if (vm is ColumnViewModel cvm)
            {
                if (this.Columns.Count <= 1)
                {
                    this.dialogService.ShowMessage(Resources.Dialog_CannotRemoveLastColumn_Message, Resources.Dialog_RemoveColumn_Title);
                    return;
                }

                this.Columns.Remove(cvm);
                this.logger.Log("Column deleted", Category.Debug, Priority.None);

                // Ask user whether or not to save items in deleted column.
                if (cvm.Items.Count <= 0 || !this.dialogService.ShowYesNo(Resources.Dialog_SaveItemsInColumn_Message, Resources.Dialog_RemoveColumn_Title)) return;

                foreach (var item in cvm.Items)
                {
                    this.Columns[0].Items.Add(item);
                }

                this.logger.Log("Items migrated to left-most column", Category.Debug, Priority.None);
            }
        }

        private void BoardPropertiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (BaseCollectionItemViewModel property in e.OldItems)
                {
                    if (property is ColumnViewModel column)
                    {
                        column.Items.CollectionChanged -= BoardPropertiesChanged;
                    }
                    property.PropertyChanged -= BoardPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (BaseCollectionItemViewModel property in e.NewItems)
                {
                    if (property is ColumnViewModel column)
                    {
                        foreach (var item in column.Items) item.PropertyChanged += BoardPropertyChanged;
                        column.Items.CollectionChanged += BoardPropertiesChanged;
                    }
                    property.PropertyChanged += BoardPropertyChanged;
                }
            }

            if (e.OldItems != null || e.NewItems != null)
            {
                this.SaveBoard();
            }
        }

        private void BoardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as BaseCollectionItemViewModel;

            if (e.PropertyName == "OptionsOpen" && vm.OptionsOpen)
            {
                this.OpenedOptions(vm);
                return;
            }
            else if (e.PropertyName == "Delete")
            {
                this.DeleteBaseCollection(vm);
                return;
            }

            this.SaveBoard();
        }

        private void OnClosing()
        {
            this.logger.Log("Stacket closing", Category.Debug, Priority.None);

            Application.Current.Shutdown();
        }
    }
}