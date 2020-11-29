using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using KanbanBoard.Presentation.ViewModels;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DragHandleBehavior : DefaultDragHandler
    {
        private bool cancelled;

        public delegate void DragEventHandler();

        public event DragEventHandler DragStarted;

        public Point DragPosition { get; private set; }

        public override void StartDrag(IDragInfo dragInfo)
        {
            var position = dragInfo.PositionInDraggedItem;
            this.DragPosition = new Point(position.X / dragInfo.VisualSourceItem.RenderSize.Width, position.Y / dragInfo.VisualSourceItem.RenderSize.Height);

            this.DragStarted?.Invoke();

            dragInfo.Effects = DragDropEffects.Move;
            dragInfo.Data = dragInfo.SourceItem;

            //Only perform the fancy drag on Items (for now).
            if (dragInfo.SourceItem is ItemViewModel)
            {
                //Set the minimum width to the actual width. We have to do this as once its removed from the control it will resize and the adorner will take its size.
                ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = ((FrameworkElement)dragInfo.VisualSourceItem).ActualWidth;

                //Remove the source item.
                //If we don't do this the seperator item created in the DropHandler will appear as an extra space in the original column and it looks broken.
                var item = (ItemViewModel)dragInfo.SourceItem;
                ((IList<ItemViewModel>)((ItemsControl)dragInfo.VisualSource).ItemsSource).Remove(item);
            }
        }

        public override void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is ItemViewModel)
            {
                //If we've cancelled the operation we need to add the item back to its original place as we removed it earlier.
                if (this.cancelled)
                {
                    var item = (ItemViewModel)dragInfo.SourceItem;
                    ((IList<ItemViewModel>)((ItemsControl)dragInfo.VisualSource).ItemsSource).Insert(dragInfo.SourceIndex, item);
                    this.cancelled = false;
                }

                //Set back the minimum width so columns can be added and the items will size accordingly.
                ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = 0;
            }
            base.DragDropOperationFinished(operationResult, dragInfo);
        }

        public override void DragCancelled()
        {
            this.cancelled = true;
            base.DragCancelled();
        }
    }
}