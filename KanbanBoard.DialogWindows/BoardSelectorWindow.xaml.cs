using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KanbanBoard {
   /// <summary>
   /// Interaction logic for BoardSelectorWindow.xaml
   /// </summary>
   public partial class BoardSelectorWindow : Window {
      public BoardSelectorWindow(string currentBoard) {
         InitializeComponent();
         this.DataContext = new BoardSelectorViewModel(this, currentBoard);
      }

      private void WindowMouseDown(object sender, MouseButtonEventArgs e) {
         if (e.LeftButton == MouseButtonState.Pressed) {
            this.DragMove();
         }
      }
   }
}
