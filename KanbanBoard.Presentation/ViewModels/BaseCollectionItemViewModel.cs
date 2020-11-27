using System;
using Kanban.Core.Events;
using Prism.Events;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.ViewModels
{
    public abstract class BaseCollectionItemViewModel : BindableBase
    {
        private bool optionsOpen;
        private string title;

        protected BaseCollectionItemViewModel(Guid? id, string title)
        {
            this.Id = id ?? Guid.NewGuid();
            this.title = title;
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

        public bool OptionsOpen
        {
            get => this.optionsOpen;
            set => this.SetProperty(ref this.optionsOpen, value);
        }
    }
}