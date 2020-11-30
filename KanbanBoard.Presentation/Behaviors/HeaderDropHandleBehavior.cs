using GongSolutions.Wpf.DragDrop;
using KanbanBoard.Presentation.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace KanbanBoard.Presentation.Behaviors
{
    public class HeaderDropHandleBehavior : DefaultDropHandler
    {
        public IList<ItemViewModel> items;

        public HeaderDropHandleBehavior(IList<ItemViewModel> items)
        {
            this.items = items;
        }

        public override void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo.SourceItem is ItemViewModel item)
            {
                this.items.Add(item);
            }
        }
    }
}
