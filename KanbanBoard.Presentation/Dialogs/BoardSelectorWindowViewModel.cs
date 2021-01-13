using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kanban.Core.Events;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class BoardSelectorWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IDialogService dialogService;
        private readonly ILoggerFacade logger;
        private readonly IFTPService ftpService;

        private string selectedBoard;

        public BoardSelectorWindowViewModel(
            IEventAggregator eventAggregator,
            IDialogService dialogService,
            ILoggerFacade logger,
            IFTPService ftpService,
            Action closeDialog)
        {
            this.dialogService = dialogService;
            this.logger = logger;
            this.ftpService = ftpService;
            this.closeDialog = closeDialog;

            if (this.IsOnline && !this.ftpService.IsConnected)
            {
                SetConnection(this.IsOnline);
            }
            else
            {
                this.PopulateBoardFiles(this.ftpService.IsConnected);
            }

            this.CloseDialogCommand = new DelegateCommand(this.CloseDialog);
            this.NewButtonCommand = new DelegateCommand(this.NewButton);
            this.OpenButtonCommand = new DelegateCommand(this.OpenButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
            this.DeleteButtonCommand = new DelegateCommand(this.DeleteButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
            this.ToggleConnectionCommand = new DelegateCommand(this.ToggleConnection);

            eventAggregator.GetEvent<FtpConnectionChangedEvent>().Subscribe((o) =>
            {
                this.PopulateBoardFiles(o);
                RaisePropertyChanged(nameof(IsOnline));
            });
        }

        public string BoardLocation { get; private set; }

        public string SelectedBoard
        {
            get => this.selectedBoard;
            set => this.SetProperty(ref this.selectedBoard, value);
        }

        public bool IsOnline => Settings.Default.IsOnline;

        public ObservableCollection<string> BoardFiles { get; } = new ObservableCollection<string>();

        public ICommand CloseDialogCommand { get; }
        public ICommand NewButtonCommand { get; }
        public ICommand OpenButtonCommand { get; }
        public ICommand DeleteButtonCommand { get; }
        public ICommand ToggleConnectionCommand { get; }

        private void PopulateBoardFiles(bool online)
        {
            this.BoardFiles.Clear();

            IEnumerable<string> files;

            if (online)
            {
                files = ftpService.GetAllFiles().Select(Path.GetFileNameWithoutExtension);
            }
            else
            {
                files = Directory.GetFiles(FileLocations.BoardFileStorageLocation);
            }

            this.BoardFiles.AddRange(files.Select(Path.GetFileNameWithoutExtension));
            this.logger.Log("Populated board files list", Category.Debug, Priority.None);
        }

        private void NewButton()
        {
            this.logger.Log("New board requested", Category.Debug, Priority.None);
            var input = this.dialogService.GetInput(Resources.Dialog_NewBoard_Message, Resources.Dialog_NewBoard_Title);
            if (string.IsNullOrEmpty(input)) return;

            if (IsOnline)
            {
                this.BoardLocation = input + Resources.BoardFileExtension;
            }
            else
            {
                this.BoardLocation = Path.Combine(FileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);
            }

            this.CloseDialog();
        }

        private void OpenButton()
        {
            this.logger.Log("Open board requested", Category.Debug, Priority.None);
            if (IsOnline)
            {
                this.BoardLocation = this.SelectedBoard + Resources.BoardFileExtension;
            }
            else
            {
                this.BoardLocation = Path.Combine(FileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension);
            }
            this.CloseDialog();
        }

        private void DeleteButton()
        {
            var result = this.dialogService.ShowYesNo(Resources.Dialog_RemoveBoard_Message, Resources.Dialog_RemoveBoard_Title);
            if (!result.HasValue || !result.Value) return;
            if (IsOnline)
            {
                this.ftpService.DeleteFile(this.SelectedBoard + Resources.BoardFileExtension);
            }
            else
            {
                File.Delete(Path.Combine(FileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension));
            }
            this.BoardFiles.Remove(this.SelectedBoard);
            this.logger.Log("Board deleted", Category.Debug, Priority.None);
        }

        private bool IsFileSelected()
        {
            return this.BoardFiles.Count > 0 && this.SelectedBoard != null;
        }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }

        private void ToggleConnection()
        {
            var value = !this.IsOnline;
            this.SetConnection(value);
        }

        private void SetConnection(bool value)
        {
            if (value)
            {
                this.ftpService.GoOnline();
                this.logger.Log("Connected to FTP server", Category.Debug, Priority.None);
            }
            else
            {
                this.ftpService.GoOffline();
                this.logger.Log("Disconnected from FTP server", Category.Debug, Priority.None);
            }
        }
    }
}