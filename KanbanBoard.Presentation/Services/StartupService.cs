using System.Diagnostics;
using System.IO;
using System.Windows;
using KanbanBoard.Presentation.Properties;

namespace KanbanBoard.Presentation.Services
{
    public class StartupService : IStartupService
    {
        private readonly IDialogService dialogService;
        private readonly IRegistryService registryService;

        public StartupService(IDialogService dialogService, IRegistryService registryService)
        {
            this.dialogService = dialogService;
            this.registryService = registryService;
        }

        public void Initialize()
        {
            if (!Settings.Default.RanOnce)
            {
                Settings.Default.RanOnce = true;
            }
            else if (!Settings.Default.AskedUserForStartup)
            {
                if (dialogService.ShowYesNo("Should Stacket start on Windows startup?", "Stacket"))
                {
                    this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName,
                        Process.GetCurrentProcess().MainModule.FileName);
                }

                Settings.Default.AskedUserForStartup = true;
            }

            Directory.CreateDirectory(FileLocations.BoardFileStorageLocation);

            if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
            {
                Settings.Default.CurrentBoard = this.dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
            }
            else if (!File.Exists(Settings.Default.CurrentBoard))
            {
                this.dialogService.ShowMessage("The board " + Path.GetFileName(Settings.Default.CurrentBoard) + " is missing.", "Missing Board File");
                Settings.Default.CurrentBoard = this.dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}