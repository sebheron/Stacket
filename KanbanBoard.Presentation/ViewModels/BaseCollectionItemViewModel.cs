﻿using System;
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

        private bool isItemEnabled;

        protected BaseCollectionItemViewModel(Guid? id, string title, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null) return;

            this.EventAggregator = eventAggregator;

            this.EventAggregator.GetEvent<IsDraggingEvent>().Subscribe((d) => this.NotDragging = !d);

            this.Id = id ?? Guid.NewGuid();
            this.title = title;
            this.NotDragging = true;
            this.IsItemEnabled = true;
        }

        public Guid Id { get; }

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

        public bool NotDragging
        {
            get => this.notDragging;
            set => this.SetProperty(ref this.notDragging, value);
        }

        public bool IsItemEnabled
        {
            get => this.isItemEnabled;
            set => this.SetProperty(ref this.isItemEnabled, value);
        }
    }
}