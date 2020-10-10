using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using KanbanBoard.Presentation.Services;

namespace KanbanBoard.Presentation.Dialogs
{
    public class BoardSelectorViewModel : BindableBase
    {
        private readonly Window dialogWindow;

        private string selectedBoard;

        public BoardSelectorViewModel(Window window, string currentBoard)
        {
            this.BoardFiles = new ObservableCollection<string>(GetFileNames(currentBoard));
            this.RaisePropertyChanged(nameof(this.BoardFiles));
            this.dialogWindow = window;

            this.CancelButtonCommand = new DelegateCommand(this.CancelButton, () => true);
            this.NewButtonCommand = new DelegateCommand(this.NewButton, () => true);
            this.OpenButtonCommand = new DelegateCommand(this.OpenButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
            this.DeleteButtonCommand = new DelegateCommand(this.DeleteButton, this.IsFileSelected).ObservesProperty(() => this.SelectedBoard);
        }

        public string SelectedBoard
        {
            get => this.selectedBoard;
            set => this.SetProperty(ref this.selectedBoard, value);
        }

        public ObservableCollection<string> BoardFiles { get; set; }

        public ICommand CancelButtonCommand { get; }
        public ICommand NewButtonCommand { get; }
        public ICommand OpenButtonCommand { get; }
        public ICommand DeleteButtonCommand { get; }

        private IEnumerable<string> GetFileNames(string currentBoard)
        {
            return Directory.GetFiles(BoardHandling.BoardFileStorageLocation).Where(x => x != currentBoard)
                .Select(Path.GetFileNameWithoutExtension);
        }

        public void CancelButton()
        {
            this.dialogWindow.DialogResult = false;
            this.dialogWindow.Close();
        }

        public void NewButton()
        {
            var input = DialogBoxService.GetInput("Name for the new board:", "New Board");
            if (string.IsNullOrEmpty(input)) return;

            this.dialogWindow.Tag = Path.Combine(BoardHandling.BoardFileStorageLocation,
                input + BoardHandling.BoardFileExtension);
            this.dialogWindow.DialogResult = true;
            this.dialogWindow.Close();
        }

        public void OpenButton()
        {
            this.dialogWindow.Tag = Path.Combine(BoardHandling.BoardFileStorageLocation,
                SelectedBoard + BoardHandling.BoardFileExtension);
            this.dialogWindow.DialogResult = true;
            this.dialogWindow.Close();
        }

        public void DeleteButton()
        {
            if (!DialogBoxService.ShowYesNo("Are you sure want to delete this board?", "Delete board")) return;

            File.Delete(Path.Combine(BoardHandling.BoardFileStorageLocation, this.SelectedBoard + BoardHandling.BoardFileExtension));
            this.BoardFiles.Remove(this.SelectedBoard);
        }

        public bool IsFileSelected()
        {
            return this.BoardFiles.Count > 0 && this.SelectedBoard != null;
        }
    }
}