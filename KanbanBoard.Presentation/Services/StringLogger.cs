using System;
using System.Diagnostics;
using System.Text;

namespace KanbanBoard.Presentation.Services
{
    public class StringLogger : IStringLogger
    {
        public readonly StringBuilder Builder;

        public StringLogger()
        {
            Builder = new StringBuilder();
        }

        public void Log(string message)
        {
            Debug.WriteLine(message);
            Builder.Append(message);
            Builder.Append("%0D%0A");
        }
    }
}