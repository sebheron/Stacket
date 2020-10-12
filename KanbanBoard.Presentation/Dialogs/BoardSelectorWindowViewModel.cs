using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using KanbanBoard.Presentation.Services;
using KanbanBoard.Properties;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class BoardSelectorWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private string selectedBoard;

        public BoardSelectorWindowViewModel(Action closeDialog)
        {
            this.closeDialog = closeDialog;

            this.BoardFiles.AddRange(GetFileNames(Settings.Default.CurrentBoard));

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

        private static IEnumerable<string> GetFileNames(string currentBoard)
        {
            return Directory.GetFiles(BoardHandling.BoardFileStorageLocation).Select(Path.GetFileNameWithoutExtension);
        }

        private void NewButton()
        {
            var input = DialogBoxService.GetInput("Name for the new board:", "New Board");
            if (string.IsNullOrEmpty(input)) return;

            this.BoardLocation = Path.Combine(BoardHandling.BoardFileStorageLocation, input + BoardHandling.BoardFileExtension);
            this.CloseDialog();
        }

        private void OpenButton()
        {
            this.BoardLocation = Path.Combine(BoardHandling.BoardFileStorageLocation, this.SelectedBoard + BoardHandling.BoardFileExtension);
            this.CloseDialog();
        }

        private void DeleteButton()
        {
            if (!DialogBoxService.ShowYesNo("Are you sure want to delete this board?", "Delete board")) return;

            File.Delete(Path.Combine(BoardHandling.BoardFileStorageLocation, this.SelectedBoard + BoardHandling.BoardFileExtension));
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