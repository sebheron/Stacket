using KanbanBoard.DialogWindows;
using KanbanBoard.Objects;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KanbanBoard {
   public class BoardSelectorViewModel : BindableBase {

      private Window dialogWindow;

      private string selectedBoard;

      public string SelectedBoard {
         get => selectedBoard;
         set {
            SetProperty(ref selectedBoard, value);
         }
      }

      public ObservableCollection<string> BoardFiles { get; set; }

      public DelegateCommand CancelButtonCommand => new DelegateCommand(CancelButton, delegate () { return true; });
      public DelegateCommand NewButtonCommand => new DelegateCommand(NewButton, delegate () { return true; });
      public DelegateCommand OpenButtonCommand => new DelegateCommand(OpenButton, IsFileSelected).ObservesProperty(() => SelectedBoard);
      public DelegateCommand DeleteButtonCommand => new DelegateCommand(DeleteButton, IsFileSelected).ObservesProperty(() => SelectedBoard);

      private IEnumerable<string> GetFileNames(string currentBoard) {
         return Directory.GetFiles(BoardHandling.BoardFileStorageLocation).Where(x => x != currentBoard).Select(x => Path.GetFileNameWithoutExtension(x));
      }

      public BoardSelectorViewModel(Window window, string currentBoard) {
         BoardFiles = new ObservableCollection<string>(GetFileNames(currentBoard));
         RaisePropertyChanged(nameof(BoardFiles));
         dialogWindow = window;
      }

      public void CancelButton() {
         dialogWindow.DialogResult = false;
         dialogWindow.Close();
      }

      public void NewButton() {
         string input = DialogBoxService.GetInput("Name for the new board:", "New Board");
         if (input == string.Empty || input == null) {
            return;
         } else {
            dialogWindow.Tag = Path.Combine(BoardHandling.BoardFileStorageLocation, input + BoardHandling.BoardFileExtension);
         }
         dialogWindow.DialogResult = true;
         dialogWindow.Close();
      }

      public void OpenButton() {
         dialogWindow.Tag = Path.Combine(BoardHandling.BoardFileStorageLocation, SelectedBoard + BoardHandling.BoardFileExtension);
         dialogWindow.DialogResult = true;
         dialogWindow.Close();
      }

      public void DeleteButton() {
         if (DialogBoxService.ShowYesNo("Are you sure want to delete this board?", "Delete board")) {
            File.Delete(Path.Combine(BoardHandling.BoardFileStorageLocation, SelectedBoard + BoardHandling.BoardFileExtension));
            BoardFiles.Remove(SelectedBoard);
         }
      }

      public bool IsFileSelected() {
         return BoardFiles.Count > 0 && SelectedBoard != null;
      }
   }
}
