using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class BoardSelectorWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IDialogService dialogService;
        private readonly IStringLogger logger;

        private string[] fileLocations;

        private string selectedBoard;

        public BoardSelectorWindowViewModel(IDialogService dialogService, IStringLogger logger, Action closeDialog)
        {
            this.dialogService = dialogService;
            this.logger = logger;
            this.closeDialog = closeDialog;

            this.PopulateBoardFiles();

            this.CloseDialogCommand = new DelegateCommand(this.CloseDialog);
            this.NewButtonCommand = new DelegateCommand(this.NewButton);
            this.OpenButtonCommand = new DelegateCommand(this.OpenButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
            this.DeleteButtonCommand = new DelegateCommand(this.DeleteButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
        }

        public string BoardLocation { get; private set; }

        public string SelectedBoard
        {
            get => this.selectedBoard;
            set => this.SetProperty(ref this.selectedBoard, value);
        }

        public ObservableCollection<string> BoardFiles { get; } = new ObservableCollection<string>();

        public ICommand CloseDialogCommand { get; }
        public ICommand NewButtonCommand { get; }
        public ICommand OpenButtonCommand { get; }
        public ICommand DeleteButtonCommand { get; }

        private void PopulateBoardFiles()
        {
            this.fileLocations = Directory.GetFiles(FileLocations.BoardFileStorageLocation);
            this.BoardFiles.AddRange(this.fileLocations.Where(file => file != Settings.Default.CurrentBoard).Select(Path.GetFileNameWithoutExtension));
            this.logger.Log("Populated board files list");
        }

        private void NewButton()
        {
            this.logger.Log("New board requested");
            var input = this.dialogService.GetInput(Resources.Dialog_NewBoard_Message, Resources.Dialog_NewBoard_Title);
            while (this.fileLocations.Select(Path.GetFileNameWithoutExtension).Contains(input))
            {
                input = this.dialogService.GetInput(Resources.Dialog_NewBoard_Error + Environment.NewLine + Resources.Dialog_NewBoard_Message, Resources.Dialog_NewBoard_Title);
            }
            if (string.IsNullOrEmpty(input)) return;
            this.BoardLocation = Path.Combine(FileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);

            this.CloseDialog();
        }

        private void OpenButton()
        {
            this.logger.Log("Open board requested");
            this.BoardLocation = Path.Combine(FileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension);
            this.CloseDialog();
        }

        private void DeleteButton()
        {
            var result = this.dialogService.ShowYesNo(Resources.Dialog_RemoveBoard_Message, Resources.Dialog_RemoveBoard_Title);
            if (!result.HasValue || !result.Value) return;

            File.Delete(Path.Combine(FileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension));
            this.BoardFiles.Remove(this.SelectedBoard);
            this.logger.Log("Board deleted");
        }

        private bool IsFileSelected()
        {
            return this.BoardFiles.Count > 0 && this.SelectedBoard != null;
        }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }
    }
}