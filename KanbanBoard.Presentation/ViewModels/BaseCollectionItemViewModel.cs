using System;
using Kanban.Core.Events;
using Prism.Events;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public abstract class BaseCollectionItemViewModel : BindableBase
    {
        protected readonly IEventAggregator EventAggregator;

        private string title;

        private bool notDragging;

        protected BaseCollectionItemViewModel(Guid? id, string title, IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;

            this.EventAggregator.GetEvent<IsDraggingEvent>().Subscribe((d) => this.NotDragging = !d);

            this.Id = id ?? Guid.NewGuid();
            this.title = title;
            this.NotDragging = true;

            this.PropertyChanged += ItemPropertyChanged;
        }

        private void ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
        }

        public Guid Id { get; }

        public string Title
        {
            get => title;
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                this.SetProperty(ref title, value);
            }
        }

        public bool NotDragging
        {
            get => this.notDragging;
            set => this.SetProperty(ref this.notDragging, value);
        }
    }
}