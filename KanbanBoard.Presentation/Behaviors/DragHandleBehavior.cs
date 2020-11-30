﻿using System.Windows;
using GongSolutions.Wpf.DragDrop;
using KanbanBoard.Presentation.ViewModels;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DragHandleBehavior : DefaultDragHandler
    {
        public delegate void DragEventHandler();

        public event DragEventHandler DragStarted;

        public Point DragPosition { get; private set; }

        public override void StartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is ItemViewModel item)
            {
                item.NewlyCreatedItem = false;
            }

            var position = dragInfo.PositionInDraggedItem;
            this.DragPosition = new Point(position.X / dragInfo.VisualSourceItem.RenderSize.Width, position.Y / dragInfo.VisualSourceItem.RenderSize.Height);

            this.DragStarted?.Invoke();

            dragInfo.Effects = DragDropEffects.Move;
            dragInfo.Data = dragInfo.SourceItem;
        }
    }
}