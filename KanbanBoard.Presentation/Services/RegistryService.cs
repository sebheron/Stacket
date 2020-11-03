using Microsoft.Win32;
using Prism.Logging;
using System;

namespace KanbanBoard.Presentation.Services
{
    public class RegistryService : IRegistryService
    {
        private readonly ILoggerFacade logger;

        public RegistryService(ILoggerFacade logger)
        {
            this.logger = logger;
        }

        public object GetValue(string location, string name)
        {
            this.logger.Log("Registry value requested", Category.Debug, Priority.None);
            return Registry.CurrentUser.OpenSubKey(location, false).GetValue(name);
        }

        public void SetValue(string location, string name, object value)
        {
            this.logger.Log("Registry value set", Category.Debug, Priority.None);
            Registry.CurrentUser.OpenSubKey(location, true).SetValue(name, value);
        }

        public void DeleteValue(string location, string name)
        {
            this.logger.Log("Registry value deleted", Category.Debug, Priority.None);
            Registry.CurrentUser.OpenSubKey(location, true).DeleteValue(name);
        }
    }
}