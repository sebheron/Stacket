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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KanbanBoard.Presentation.UI
{
    /// <summary>
    /// Interaction logic for CustomUserControl.xaml
    /// </summary>
    public partial class CustomItemsControl : ItemsControl
    {
        public CustomItemsControl()
        {
            InitializeComponent();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Move)
            {
                Mouse.SetCursor((Cursor)this.Resources["GrabCursor"]);
            }
            e.Handled = true;
        }
    }
}
