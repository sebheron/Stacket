using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KanbanBoard {

   public partial class Board : Window {

      private BoardViewModel viewModel;

      public Board() {
         this.DataContext = viewModel = new BoardViewModel();
         InitializeComponent();
      }

      private void Exit_Click(object sender, RoutedEventArgs e) {
         viewModel.BoardInformation.Save();
         WindowSinker.GetSinker(this).Dispose();
         Application.Current.Shutdown();
      }
   }
}
