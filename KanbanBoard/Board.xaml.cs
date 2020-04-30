using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KanbanBoard {

   public partial class Board : Window {

      public Board() {
         BoardViewModel boardViewModel = new BoardViewModel();
         this.Closing += boardViewModel.OnClosing;
         this.DataContext = boardViewModel;
         InitializeComponent();
      }

      private void Exit_Click(object sender, RoutedEventArgs e) {
         this.Close();
      }
   }
}
