using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using KanbanBoard.Logic.Properties;
using Onova;
using Onova.Models;
using Onova.Services;

namespace KanbanBoard.Presentation.Services
{
    public class StartupService : IStartupService
    {
        private readonly IDialogService dialogService;
        private readonly IRegistryService registryService;
        private readonly IStringLogger logger;

        private IPackageResolver resolver;
        private IPackageExtractor extractor;

        public StartupService(IDialogService dialogService, IRegistryService registryService, IStringLogger logger)
        {
            this.dialogService = dialogService;
            this.registryService = registryService;
            this.logger = logger;
            this.resolver = new WebPackageResolver(new HttpClient(), "https://swegrock.github.io/stacket/update.txt");
            this.extractor = new ZipPackageExtractor();
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
                dialogService.ShowMessage("Stacket is already running.", Resources.Stacket);
                return true;
            }
            return false;
        }

        private void AskUserToStartOnStartup()
        {
            if (!Settings.Default.RanOnce)
            {
                Settings.Default.Upgrade();
                Settings.Default.RanOnce = true;
                this.dialogService.ShowStartup();
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
                    Task.Run(CheckForUpdates);
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

        private async void CheckForUpdates()
        {
            using (var manager = new UpdateManager(this.resolver, this.extractor))
            {
                var result = await manager.CheckForUpdatesAsync();
                if (result.CanUpdate)
                {
                    bool? update = false;
                    Application.Current.Dispatcher.Invoke(() => update = dialogService.ShowYesNo($"An update for Stacket (V {result.LastVersion.ToString()}) is available, download and install it?", Resources.Stacket));
                    if (!update.HasValue || !update.Value) return;

                    await manager.PrepareUpdateAsync(result.LastVersion);
                    manager.LaunchUpdater(result.LastVersion);
                    Environment.Exit(0);
                }
            }
        }
    }
}