using System;
using System.Diagnostics;
using System.Windows.Input;
using KanbanBoard.Presentation.Properties;
using KanbanBoard.Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class SettingsWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private readonly IRegistryService registryService;
        private bool startOnStartup;
        private bool lockToggle;

        public SettingsWindowViewModel(Action closeDialog, IRegistryService registryService)
        {
            this.closeDialog = closeDialog;

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.AcceptCommand = new DelegateCommand(this.Accept);
            this.registryService = registryService;
            this.LockToggle = Properties.Settings.Default.LockToggle;
            this.StartOnStartup = !string.IsNullOrEmpty(registryService.GetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName) as string);
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
            this.closeDialog.Invoke();
        }

        private void Accept()
        {
            if (this.StartOnStartup)
            {
                this.registryService.SetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName, Process.GetCurrentProcess().MainModule.FileName);
            }
            else if (this.registryService.GetValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName) != null)
            {
                this.registryService.DeleteValue(Resources.StartupRegistryLocation, Resources.StartupRegistryName);
            }

            Properties.Settings.Default.LockToggle = lockToggle;
            Properties.Settings.Default.Save();

            this.closeDialog.Invoke();
        }
    }
}