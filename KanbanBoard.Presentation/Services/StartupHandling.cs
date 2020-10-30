using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KanbanBoard.Presentation.Properties;

namespace KanbanBoard.Presentation.Services
{
    public class StartupHandling
    {
        public static void Initialize(IDialogService dialogService, IRegistryService registryService)
        {
            Settings.Default.PropertyChanged += SaveSettings;
            AskUserToAutoStartApp(dialogService, registryService);
            Directory.CreateDirectory(FileLocations.BoardFileStorageLocation);
            OpenLastBoard(dialogService);
        }

        private static void SaveSettings(object sender, PropertyChangedEventArgs e)
        {
            Settings.Default.Save();
        }

        private static void AskUserToAutoStartApp(IDialogService dialogService, IRegistryService registryService)
        {
            if (!Settings.Default.RanOnce)
            {
                Settings.Default.RanOnce = true;
            }
            else if (!Settings.Default.AskedUserForStartup)
            {
                if (dialogService.ShowYesNo("Should Stacket start on Windows startup?", "Stacket"))
                {
                    registryService.SetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName,
                        Process.GetCurrentProcess().MainModule.FileName);
                }

                Settings.Default.AskedUserForStartup = true;
            }
        }

        private static void OpenLastBoard(IDialogService dialogService)
        {
            if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
            {
                Settings.Default.CurrentBoard = dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
            }
            else if (!File.Exists(Settings.Default.CurrentBoard))
            {
                dialogService.ShowMessage("The board " + Path.GetFileName(Settings.Default.CurrentBoard) + " is missing.", "Missing Board File");
                Settings.Default.CurrentBoard = dialogService.SelectBoard();
                if (string.IsNullOrEmpty(Settings.Default.CurrentBoard))
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}