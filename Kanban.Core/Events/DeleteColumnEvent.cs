using System;
using Prism.Events;

namespace Kanban.Core.Events
{
    public class DeleteColumnEvent : PubSubEvent<Guid>
    {
    }
}