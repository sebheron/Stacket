using System;
using Prism.Events;

namespace Kanban.Core.Events
{
    public class DeleteItemEvent : PubSubEvent<Guid>
    {
    }
}