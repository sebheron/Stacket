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
using System.Threading.Tasks;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly IColumnViewModelFactory columnFactory;
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IFTPService ftpService;

        private string filePath => Settings.Default.CurrentBoard;
        private bool loadEnabled = true;
        private bool newEnabled = true;
        private bool isBoardShown;
        private bool loadInProgress;
        private bool disableBackground;
        private DateTime modifiedDate;

        public BoardViewModel(
            IColumnViewModelFactory columnFactory,
            IDialogService dialogService,
            ILoggerFacade logger,
            IEventAggregator eventAggregator,
            IFTPService ftpService)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<DeleteColumnEvent>().Subscribe(this.DeleteColumn);
            this.eventAggregator.GetEvent<RequestSaveEvent>().Subscribe(this.SaveBoard);
            this.eventAggregator.GetEvent<IsDraggingEvent>().Subscribe((d) => this.DisableBackground = d);

            this.columnFactory = columnFactory;
            this.logger = logger;
            this.dialogService = dialogService;
            this.ftpService = ftpService;

            this.DragHandler = new DragHandleBehavior(this.eventAggregator);
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

        public bool DisableBackground
        {
            get => this.disableBackground;
            set => SetProperty(ref this.disableBackground, value);
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

        public bool IsBoardShown
        {
            get => this.isBoardShown;
            set => SetProperty(ref this.isBoardShown, value);
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

            this.IsBoardShown = false;

            this.Columns.CollectionChanged += this.ColumnsChanged;
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

            if (Settings.Default.IsOnline)
            {
                Settings.Default.CurrentBoard = input + Resources.BoardFileExtension;
            }
            else
            {
                Settings.Default.CurrentBoard = Path.Combine(FileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);
            }

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

            List<string> lines = new List<string>();

            if (Settings.Default.IsOnline)
            {
                lines.AddRange(this.ftpService.ReadAllLines(this.filePath));
            }
            else
            {
                if (File.Exists(this.filePath))
                {
                    // For each version in future releases migration will occur here! Via Assembly information check.
                    lines.AddRange(File.ReadAllLines(this.filePath));
                }
            }

            if (lines.Count <= 0)
            {
                this.CreateDefaultBoard();
            }
            else
            {
                for (var i = 2; i < lines.Count; i++)
                {
                    this.Columns.Add(this.columnFactory.Load(lines.ElementAt(i)));
                }
            }

            this.loadInProgress = false;
        }

        public void CreateDefaultBoard()
        {
            this.logger.Log("File doesn't exist, creating default board", Category.Debug, Priority.None);

            this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_New));
            this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_InProgress));
            this.Columns.Add(this.columnFactory.CreateColumn(Resources.ColumnName_Done));

            this.loadInProgress = false;

            // Create the board file with default data.
            this.SaveBoard();
        }

        public void SaveBoard()
        {
            if (this.loadInProgress || string.IsNullOrEmpty(this.filePath)) return;

            var boardData = new List<string> { Assembly.GetExecutingAssembly().GetName().Version.ToString(), string.Empty };

            boardData.AddRange(this.Columns.Select(column => column.ToString()));

            if (Settings.Default.IsOnline)
            {
                this.ftpService.WriteAllLines(this.filePath, boardData.ToArray());
            }
            else
            {
                File.WriteAllLines(this.filePath, boardData.ToArray());
            }

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

        private void DeleteColumn(Guid columnId)
        {
            var columnToDelete = this.Columns.FirstOrDefault(column => column.Id == columnId);
            if (columnToDelete == null) return;

            if (this.Columns.Count <= 1)
            {
                this.dialogService.ShowMessage(Resources.Dialog_CannotRemoveLastColumn_Message, Resources.Dialog_RemoveColumn_Title);
                return;
            }

            var items = columnToDelete.Items;

            var result = items.Count > 0 ? this.dialogService.ShowYesNo(Resources.Dialog_SaveItemsInColumn_Message, Resources.Dialog_RemoveColumn_Title) : false;
            if (!result.HasValue) return; //Cancelled selected on the dialog.

            this.Columns.Remove(columnToDelete);
            this.logger.Log("Column deleted", Category.Debug, Priority.None);

            if (!result.Value) return; //No selected on the dialog or there's no items to move across.

            foreach (var item in items)
            {
                this.Columns[0].Items.Add(item);
            }

            this.logger.Log("Items migrated to left-most column", Category.Debug, Priority.None);
        }

        private void ColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.ColumnWidth));

            this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        private void OnClosing()
        {
            this.logger.Log("Stacket closing", Category.Debug, Priority.None);

            Application.Current.Shutdown();
        }
    }
}