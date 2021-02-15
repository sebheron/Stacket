﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using Kanban.Core.Events;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DragHandleBehavior : DefaultDragHandler
    {
        private readonly IEventAggregator eventAggregator;

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

            //Remove newly created tag.
            if (dragInfo.SourceItem is ItemViewModel item)
            {
                item.NewlyCreatedItem = false;
            }

            //Set the minimum width to the actual width. We have to do this as once its removed from the control it will resize and the adorner will take its size.
            ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = ((FrameworkElement)dragInfo.VisualSourceItem).ActualWidth;

            //Remove the source item.
            //If we don't do this the seperator item created in the DropHandler will appear as an extra space in the original column and it looks broken.
            ((IList)((ItemsControl)dragInfo.VisualSource).ItemsSource).Remove(dragInfo.SourceItem);

            //Tell the rest of the application we're dragging.
            this.eventAggregator.GetEvent<IsDraggingEvent>().Publish(true);

            //The blessed gong doesn't feature a way of knowing if we've dropped in the final method call so we've made our own.
            dropped = false;
        }

        public override void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {

            //If we haven't been dropped we need to return back to our original place.
            if (!this.dropped)
            {
                sourceItems.Insert(dragInfo.SourceIndex, (BaseCollectionItemViewModel)dragInfo.SourceItem);
                this.dropped = false;

                //In the case of columns the separator will still be present too.
                for (int i = 0; i < sourceItems.Count; i++)
                {
                    if (!((BaseCollectionItemViewModel)sourceItems[i]).IsItemEnabled) {
                        sourceItems.RemoveAt(i);
                        break;
                    }
                }
            }

            //Turn options off.
            if (dragInfo.SourceItem is ItemViewModel item)
            {
                item.OptionsShown = false;
            }

            //Set back the minimum width so columns can be added and the items will size accordingly.
            ((FrameworkElement)dragInfo.VisualSourceItem).MinWidth = 0;

            //Tell the application we've stopped dragging and save.
            this.eventAggregator.GetEvent<IsDraggingEvent>().Publish(false);
            this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
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