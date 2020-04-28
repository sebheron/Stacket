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
   /// Interaction logic for InputBox.xaml
   /// </summary>
   public partial class DialogBoxWindow : Window {

      public DialogBoxWindow(string text, string caption) {
         InitializeComponent();
         this.DataContext = new DialogBoxViewModel(this, text, caption);
      }
   }
}
