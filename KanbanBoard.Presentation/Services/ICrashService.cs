using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoard.Presentation.Services
{
    public interface ICrashService
    {
        void SendCrash(string emailBody);
    }
}