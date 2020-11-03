using System.Diagnostics;
using System.IO;
using KanbanBoard.Presentation.Properties;
using Prism.Logging;

namespace KanbanBoard.Presentation.Services
{
    public class StartupService : IStartupService
    {
        private readonly IDialogService dialogService;
        private readonly IRegistryService registryService;
        private readonly ILoggerFacade logger;

        public StartupService(IDialogService dialogService, IRegistryService registryService, ILoggerFacade logger)
        {
            this.dialogService = dialogService;
            this.registryService = registryService;
            this.logger = logger;
        }

        public bool Initialize()
        {
            this.logger.Log("Initializing startup", Category.Debug, Priority.None);
            if (this.IsAlreadyRunning()) return false;
            this.AskUserToStartOnStartup();
            this.CreateBoardStorageFolder();
            return this.CheckIfBoardIsOpenOrShowOpenBoardDialog();
        }

        private bool IsAlreadyRunning()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                dialogService.ShowMessage("Stacket is already running", Resources.Stacket);
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
                if (dialogService.ShowYesNo(Resources.Dialog_NewBoard_Startup, Resources.Stacket))
                {
                    this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.Stacket,
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
                this.dialogService.ShowMessage(string.Format(Resources.Dialog_MissingBoard_Message, Path.GetFileName(Settings.Default.CurrentBoard)), Resources.Dialog_MissingBoard_Title);
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