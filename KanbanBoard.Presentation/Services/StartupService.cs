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

        public bool Initialize()
        {
            if (this.IsAlreadyRunning()) return false;
            this.AskUserToStartOnStartup();
            this.CreateBoardStorageFolder();
            return this.CheckIfBoardIsOpenOrShowOpenBoardDialog();
        }

        private bool IsAlreadyRunning()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                dialogService.ShowMessage("Stacket is already running", "Stacket");
                return true;
            }
            return false;
        }

        private void AskUserToStartOnStartup()
        {
            if (!Settings.Default.RanOnce)
            {
                Settings.Default.RanOnce = true;
            }
            else if (!string.IsNullOrEmpty(Settings.Default.CurrentBoard) && !Settings.Default.AskedUserForStartup)
            {
                if (dialogService.ShowYesNo("Should Stacket start on Windows startup?", "Stacket"))
                {
                    this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName,
                        Process.GetCurrentProcess().MainModule.FileName);
                }

                Settings.Default.AskedUserForStartup = true;
            }
        }

        private void CreateBoardStorageFolder()
        {
            if (!Directory.Exists(FileLocations.BoardFileStorageLocation))
            {
                Directory.CreateDirectory(FileLocations.BoardFileStorageLocation);
            }
        }

        private bool CheckIfBoardIsOpenOrShowOpenBoardDialog()
        {
            if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
            {
                Settings.Default.CurrentBoard = this.dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    return false;
                }
            }
            else if (!File.Exists(Settings.Default.CurrentBoard))
            {
                this.dialogService.ShowMessage("The board " + Path.GetFileName(Settings.Default.CurrentBoard) + " is missing.", "Missing Board File");
                Settings.Default.CurrentBoard = this.dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    return false;
                }
            }

            return true;
        }
    }
}