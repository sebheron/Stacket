using System;
using Prism.Events;

namespace Kanban.Core.Events
{
    public class OpenOptionsEvent : PubSubEvent<Guid>
    {
    }
}