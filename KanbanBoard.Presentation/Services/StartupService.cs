using System;
using System.Diagnostics;
using System.IO;
using AutoUpdaterDotNET;
using KanbanBoard.Logic.Properties;

namespace KanbanBoard.Presentation.Services
{
    public class StartupService : IStartupService
    {
        private readonly IDialogService dialogService;
        private readonly IRegistryService registryService;
        private readonly IStringLogger logger;

        public StartupService(IDialogService dialogService, IRegistryService registryService, IStringLogger logger)
        {
            this.dialogService = dialogService;
            this.registryService = registryService;
            this.logger = logger;
        }

        public bool Initialize()
        {
            this.logger.Log("Initializing startup");
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
            else if (!string.IsNullOrEmpty(Settings.Default.CurrentBoard))
            {
                if (!Settings.Default.AskedUserForStartup)
                {
                    var result = this.dialogService.ShowYesNo(Resources.Dialog_Startup_Message, Resources.Stacket);
                    if (!result.HasValue) return;

                    if (result.Value)
                    {
                        this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.Stacket,
                                Process.GetCurrentProcess().MainModule.FileName);
                    }

                    Settings.Default.AskedUserForStartup = true;
                }
                else
                {
                    AutoUpdater.InstallationPath = AppDomain.CurrentDomain.BaseDirectory;
                    AutoUpdater.Start("https://swegrock.github.io/stacket/update.xml");
                }
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