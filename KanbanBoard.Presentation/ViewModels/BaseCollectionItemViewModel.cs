using System;
using Kanban.Core.Events;
using KanbanBoard.Presentation.Behaviors;
using Prism.Events;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public class BaseCollectionItemViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private string title;

        public BaseCollectionItemViewModel(Guid id, string title, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.DragHandler = new DragHandleBehavior();
            this.DragHandler.DragStarted += () => this.RaisePropertyChanged(nameof(this.DragHandler));

            this.Id = id;
            this.title = title;
        }

        public DragHandleBehavior DragHandler { get; }

        public Guid Id { get; }

        public string Title
        {
            get => title;
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                this.SetProperty(ref title, value);
                this.eventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }
    }
}