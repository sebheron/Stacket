using GongSolutions.Wpf.DragDrop;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;
using System.Collections.Generic;
using System.Windows;

namespace KanbanBoard.Presentation.Behaviors
{
    public class HeaderDropHandleBehavior : DefaultDropHandler
    {
        private ItemViewModel separator;

        public IList<ItemViewModel> items;

        public HeaderDropHandleBehavior(IList<ItemViewModel> items)
        {
            this.items = items;
        }

        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo.SourceItem is ItemViewModel)
            {
                dropInfo.Effects = DragDropEffects.Move;
                if (this.separator == null)
                {
                    this.separator = new ItemViewModel(new EventAggregator());
                    this.separator.IsItemEnabled = false;
                    dropInfo.VisualTarget.DragLeave += DragLeave;
                }
                if (!this.items.Contains(this.separator))
                {
                    this.items.Add(this.separator);
                }
                if (this.items.IndexOf(this.separator) != this.items.Count - 1)
                {
                    this.items.Remove(this.separator);
                    this.items.Add(this.separator);
                }
            }
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
            if (this.items.Contains(this.separator))
            {
                this.items.Remove(this.separator);
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (this.items.Contains(this.separator))
            {
                this.items.Remove(this.separator);
            }
            if (dropInfo.DragInfo.SourceItem is ItemViewModel item)
            {
                this.items.Add(item);
            }
        }
    }
}
