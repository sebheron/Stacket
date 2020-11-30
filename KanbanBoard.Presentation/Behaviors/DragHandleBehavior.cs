using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;
using Kanban.Core.Events;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DragHandleBehavior : DefaultDragHandler
    {
        private IEventAggregator eventAggregator;

        private bool dropped;

        public delegate void DragEventHandler();

        public event DragEventHandler DragStarted;

        public Point DragPosition { get; private set; }

        public DragHandleBehavior(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override void StartDrag(IDragInfo dragInfo)
        {
            var position = dragInfo.PositionInDraggedItem;
            this.DragPosition = new Point(position.X / dragInfo.VisualSourceItem.RenderSize.Width, position.Y / dragInfo.VisualSourceItem.RenderSize.Height);

            this.DragStarted?.Invoke();

            dragInfo.Effects = DragDropEffects.Move;
            dragInfo.Data = dragInfo.SourceItem;

            //Only perform the fancy drag on Items (for now).
            if (dragInfo.SourceItem is ItemViewModel item)
            {
                //Turn off new in items otherwise they'll refocus.
                item.NewlyCreatedItem = false;

                //Set the minimum width to the actual width. We have to do this as once its removed from the control it will resize and the adorner will take its size.
                ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = ((FrameworkElement)dragInfo.VisualSourceItem).ActualWidth;

                //Remove the source item.
                //If we don't do this the seperator item created in the DropHandler will appear as an extra space in the original column and it looks broken.
                ((IList<ItemViewModel>)((ItemsControl)dragInfo.VisualSource).ItemsSource).Remove(item);

                //Tell the rest of the application we're dragging.
                this.eventAggregator.GetEvent<IsDraggingEvent>().Publish(true);

                //The blessed gong doesn't feature a way of knowing if we've dropped in the final method call so we've made our own.
                dropped = false;
            }
        }

        public override void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is ItemViewModel)
            {
                //If we haven't been dropped we need to return back to our original place.
                if (!this.dropped)
                {
                    var item = (ItemViewModel)dragInfo.SourceItem;
                    ((IList<ItemViewModel>)((ItemsControl)dragInfo.VisualSource).ItemsSource).Insert(dragInfo.SourceIndex, item);
                    this.dropped = false;
                }

                //Set back the minimum width so columns can be added and the items will size accordingly.
                ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = 0;

                //Tell the application we've stopped dragging and save.
                this.eventAggregator.GetEvent<IsDraggingEvent>().Publish(false);
                this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public override void DragCancelled()
        {
            //False because we're not allowed to drop here.
            this.dropped = false;
        }

        public override void Dropped(IDropInfo dropInfo)
        {
            //True because we've successfully dropped.
            this.dropped = true;
        }
    }
}