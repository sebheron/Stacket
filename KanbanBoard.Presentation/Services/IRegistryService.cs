using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoard.Presentation.Services
{
    public interface IRegistryService
    {
        void SetValue(string location, string name, object value);

        object GetValue(string location, string name);

        void DeleteValue(string location, string name);
    }
}