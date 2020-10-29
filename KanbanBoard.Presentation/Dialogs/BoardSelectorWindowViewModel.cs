using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using KanbanBoard.Presentation.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class BoardSelectorWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly Action closeDialog;

        private string selectedBoard;

        public BoardSelectorWindowViewModel(IDialogService dialogService, Action closeDialog)
        {
            this.dialogService = dialogService;
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
            this.BoardFiles.AddRange(Directory.GetFiles(BoardFileLocations.BoardFileStorageLocation).Select(Path.GetFileNameWithoutExtension));
        }

        private void NewButton()
        {
            var input = this.dialogService.GetInput("Name for the new board:", "New Board");
            if (string.IsNullOrEmpty(input)) return;

            this.BoardLocation = Path.Combine(BoardFileLocations.BoardFileStorageLocation, input + Resources.BoardFileExtension);
            this.CloseDialog();
        }

        private void OpenButton()
        {
            this.BoardLocation = Path.Combine(BoardFileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension);
            this.CloseDialog();
        }

        private void DeleteButton()
        {
            if (!this.dialogService.ShowYesNo("Are you sure want to delete this board?", "Delete board")) return;

            File.Delete(Path.Combine(BoardFileLocations.BoardFileStorageLocation, this.SelectedBoard + Resources.BoardFileExtension));
            this.BoardFiles.Remove(this.SelectedBoard);
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