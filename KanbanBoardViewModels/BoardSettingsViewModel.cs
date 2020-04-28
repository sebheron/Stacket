using IllusoryStudios.Wpf.LostControls;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;

namespace KanbanBoard {
   public class BoardSettingsViewModel : BindableBase {

      private BoardInformation loadedBoard;

      public BoardViewModel mainViewModel;

      public BoardSettingsViewModel(BoardViewModel boardViewModel, BoardInformation boardInformation) {
         mainViewModel = boardViewModel;
         loadedBoard = boardInformation;
      }
   }
}
