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

        protected BaseCollectionItemViewModel(Guid? id, string title, IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;

            this.New = !id.HasValue;
            this.Id = id ?? Guid.NewGuid();
            this.title = title;
        }

        public Guid Id { get; }
        public bool New { get; set; }

        public string Title
        {
            get => title;
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                this.SetProperty(ref title, value);
                this.EventAggregator.GetEvent<RequestSaveEvent>().Publish();
            }
        }

        public void RequestNew()
        {
            this.RaisePropertyChanged("Request New");
        }
    }
}