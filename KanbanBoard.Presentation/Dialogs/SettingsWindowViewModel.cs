using System;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

namespace KanbanBoard.Presentation.Dialogs
{
    public class SettingsWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;
        private RegistryKey reg;
        private bool startOnStartup;
        private bool lockToggle;

        public SettingsWindowViewModel(Action closeDialog) {
            this.closeDialog = closeDialog;

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.AcceptCommand = new DelegateCommand(this.Accept);

            this.reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            this.StartOnStartup = !String.IsNullOrEmpty(reg.GetValue("Stacket") as string);
            this.LockToggle = Properties.Settings.Default.LockToggle;
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

        private void Cancel() {
            this.closeDialog.Invoke();
        }

        private void Accept() {
            if (StartOnStartup)
            {
                this.reg.SetValue("Stacket", Process.GetCurrentProcess().MainModule.FileName);
            }
            else if (reg.GetValue("Stacket") != null)
            {
                this.reg.DeleteValue("Stacket");
            }

            Properties.Settings.Default.LockToggle = lockToggle;
            Properties.Settings.Default.Save();
            this.closeDialog.Invoke();
        }
    }
}
