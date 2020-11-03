using Prism.Logging;
using System;
using System.Diagnostics;
using System.Text;

namespace KanbanBoard.Presentation.Services
{
    public class StringLogger : ILoggerFacade
    {
        public readonly StringBuilder Builder;

        public StringLogger()
        {
            Builder = new StringBuilder();
        }

        public void Log(string message, Category category, Priority priority)
        {
            if (category == Category.Debug)
            {
                Debug.WriteLine(message);
            }
            Builder.Append(message);
            Builder.Append("%0D%0A");
        }
    }
}