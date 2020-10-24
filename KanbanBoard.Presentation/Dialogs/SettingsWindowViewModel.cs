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

        public SettingsWindowViewModel(Action closeDialog) {
            this.closeDialog = closeDialog;

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.AcceptCommand = new DelegateCommand(this.Accept);

            reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            StartOnStartup = !String.IsNullOrEmpty(reg.GetValue("Stacket") as string);
        }

        public bool StartOnStartup
        {
            get => this.startOnStartup;
            set => SetProperty(ref this.startOnStartup, value);
        }

        public ICommand CancelCommand { get; }
        public ICommand AcceptCommand { get; }

        private void Cancel() {
            this.closeDialog.Invoke();
        }

        private void Accept() {
            if (StartOnStartup)
            {
                reg.SetValue("Stacket", Process.GetCurrentProcess().MainModule.FileName);
            }
            else if (reg.GetValue("Stacket") != null)
            {
                reg.DeleteValue("Stacket");
            }
            this.closeDialog.Invoke();
        }
    }
}
