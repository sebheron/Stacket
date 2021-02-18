using System;
using System.Diagnostics;
using System.Windows.Input;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class SettingsWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IRegistryService registryService;
        private readonly IStringLogger logger;
        private bool startOnStartup;
        private bool lockToggle;

        public SettingsWindowViewModel(Action closeDialog, IRegistryService registryService, IStringLogger logger)
        {
            this.closeDialog = closeDialog;
            this.logger = logger;

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.AcceptCommand = new DelegateCommand(this.Accept);
            this.registryService = registryService;
            this.LockToggle = Settings.Default.LockToggle;
            this.StartOnStartup = !string.IsNullOrEmpty(registryService.GetValue(Resources.StartupRegistryLocation, Resources.Stacket) as string);
        }

        public bool StartOnStartup
        {
            get => this.startOnStartup;
            set => SetProperty(ref this.startOnStartup, value);
        }

        public bool LockToggle
        {
            get => this.lockToggle;
            set => SetProperty(ref this.lockToggle, value);
        }

        public ICommand CancelCommand { get; }
        public ICommand AcceptCommand { get; }

        private void Cancel()
        {
            this.logger.Log("Cancel selected");
            this.closeDialog.Invoke();
        }

        private void Accept()
        {
            if (this.StartOnStartup)
            {
                this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.Stacket, Process.GetCurrentProcess().MainModule.FileName);
            }
            else if (this.registryService.GetValue(Resources.StartupRegistryLocation, Resources.Stacket) != null)
            {
                this.registryService.DeleteValue(Resources.StartupRegistryLocation, Resources.Stacket);
            }

            Settings.Default.LockToggle = lockToggle;

            this.logger.Log("Ok selected");
            this.closeDialog.Invoke();
        }
    }
}