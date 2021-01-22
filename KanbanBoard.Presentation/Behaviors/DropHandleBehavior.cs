using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using KanbanBoard.Presentation.ViewModels;
using Prism.Events;

namespace KanbanBoard.Presentation.Behaviors
{
    public class DropHandleBehavior : DefaultDropHandler
    {
        private Task task;

        private int insertIndex;

        private ItemViewModel separator;

        private ItemsControl container;

        private ObservableCollection<ItemViewModel> items;

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
            if (CanAcceptData(dropInfo))
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
                        this.items = ((ObservableCollection<ItemViewModel>)this.container.ItemsSource);
                    }
                    else
                    {
                        //Make sure to add the separator if it isn't currently in the items.
                        if (!this.items.Contains(separator))
                        {
                            this.items.Add(this.separator);
                        }

                        if (this.task == null ||this.task.IsCompleted)
                        {
                            this.task = Task.Run(() => this.insertIndex = this.GetDropIndex(dropInfo.DropPosition.Y));
                        }

                        var currentIndex = this.items.IndexOf(this.separator);
                        //Get the drop index, this is usually used for the adorner.
                        if (currentIndex != this.insertIndex)
                        {
                            this.items.Move(currentIndex, this.insertIndex);
                        }
                    }
                }
                dropInfo.DropTargetAdorner = null;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {
            //Taken from DefaultDropHandler
            if (dropInfo?.DragInfo == null)
            {
                return;
            }

            var insertIndex = this.insertIndex;
            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool forceMoveBehavior = false;

            if (destinationList != null)
            {
                var objects2Insert = new List<object>();

                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy) || dropInfo.Effects.HasFlag(DragDropEffects.Link);

                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        if (o is ICloneable cloneable)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);

                    if (!cloneData && forceMoveBehavior)
                    {
                        var index = destinationList.IndexOf(o);
                        if (insertIndex > index)
                        {
                            insertIndex--;
                        }

                        Move(destinationList, index, insertIndex++);
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }
                }

                SelectDroppedItems(dropInfo, objects2Insert);
            }
            //End of taking from DefaultDropHandler
            if (this.items.Contains(separator))
            {
                this.items.Remove(separator);
            }
        }

        protected static void Move(IList list, int sourceIndex, int destinationIndex)
        {
            var method = list.GetType().GetMethod("Move", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            method.Invoke(list, new object[] { sourceIndex, destinationIndex });
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
            if (this.container != null && this.container.Items.Contains(separator))
            {
                this.items.Remove(separator);
            }
        }

        private int GetDropIndex(double mouseY)
        {
            double totalHeight = 0;
            int i = 0;
            foreach (ItemViewModel item in this.container.Items)
            {
                if (item == this.separator) continue;
                var newHeight = totalHeight + ((UIElement)this.container.ItemContainerGenerator.ContainerFromItem(item)).RenderSize.Height;
                if (newHeight < mouseY)
                {
                    totalHeight = newHeight;
                    i++;
                }
                else
                {
                    break;
                }
            }
            return i;
        }
    }
}