using Microsoft.Win32;
using System;

namespace KanbanBoard.Presentation.Services
{
    public class RegistryService : IRegistryService
    {
        private readonly IStringLogger logger;

        public RegistryService(IStringLogger logger)
        {
            this.logger = logger;
        }

        public object GetValue(string location, string name)
        {
            this.logger.Log("Registry value requested");
            return Registry.CurrentUser.OpenSubKey(location, false).GetValue(name);
        }

        public void SetValue(string location, string name, object value)
        {
            this.logger.Log("Registry value set");
            Registry.CurrentUser.OpenSubKey(location, true).SetValue(name, value);
        }

        public void DeleteValue(string location, string name)
        {
            this.logger.Log("Registry value deleted");
            Registry.CurrentUser.OpenSubKey(location, true).DeleteValue(name);
        }
    }
}