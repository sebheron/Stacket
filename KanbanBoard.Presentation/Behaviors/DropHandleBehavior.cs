using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DropHandleBehavior : DefaultDropHandler
    {
        private ItemViewModel separator;

        private ItemsControl container;

        private IList<ItemViewModel> items;

        public DropHandleBehavior()
        {
            //Create the separator, just an empty ItemViewModel.
            //Decided against passing in the real aggregator so we don't start publishing to the rest of the application.
            this.separator = new ItemViewModel(new EventAggregator());
            this.separator.IsItemEnabled = false;
        }

        public override void DragOver(IDropInfo dropInfo)
        {
            //Check to make sure we're only modifying the DragOver for items.
            var isItem = dropInfo.DragInfo.SourceItem is ItemViewModel;
            if (isItem)
            {
                //If the container is null set it to the target column.
                //Also a dragleave function will be added to hide the separator when no columns are hovered over.
                if (this.container == null)
                {
                    this.container = dropInfo.VisualTarget as ItemsControl;
                    this.container.DragLeave += DragLeave;
                }
                else
                {
                    //If the items aren't referenced, get them.
                    if (this.items == null)
                    {
                        this.items = ((IList<ItemViewModel>)this.container.ItemsSource);
                    }
                    else
                    {
                        //Make sure to add the separator if it isn't currently in the items.
                        if (!this.items.Contains(separator))
                        {
                            this.items.Add(this.separator);
                        }

                        //Get the drop index, this is usually used for the adorner.
                        var index = dropInfo.UnfilteredInsertIndex;

                        //See if the separator needs to be moved, if it does move it.
                        if (index != this.container.Items.IndexOf(this.separator))
                        {
                            this.items.Remove(this.separator);
                            this.items.Insert(index <= this.items.Count ? index : this.items.Count, this.separator);
                        }
                    }
                }
            }
            //Same as base call with excess uneeded stuff removed.
            if (CanAcceptData(dropInfo))
            {
                dropInfo.DropTargetAdorner = null;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {

            base.Drop(dropInfo);
            if (dropInfo.DragInfo.SourceItem is ItemViewModel)
            {
                this.items.Remove(separator);
            }
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
            if (this.container != null && this.container.Items.Contains(separator))
            {
                ((IList<ItemViewModel>)this.container.ItemsSource).Remove(separator);
            }
        }
    }
}
