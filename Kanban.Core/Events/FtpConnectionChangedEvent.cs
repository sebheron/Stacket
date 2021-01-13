using System;
using Prism.Events;

namespace Kanban.Core.Events
{
    public class FtpConnectionChangedEvent : PubSubEvent<bool>
    {
    }
}