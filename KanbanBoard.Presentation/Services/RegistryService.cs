using Microsoft.Win32;
using System;

namespace KanbanBoard.Presentation.Services
{
    public class RegistryService : IRegistryService
    {
        public object GetValue(string location, string name)
        {
            return Registry.CurrentUser.OpenSubKey(location, false).GetValue(name);
        }

        public void SetValue(string location, string name, object value)
        {
            Registry.CurrentUser.OpenSubKey(location, true).SetValue(name, value);
        }

        public void DeleteValue(string location, string name)
        {
            Registry.CurrentUser.OpenSubKey(location, true).DeleteValue(name);
        }
    }
}