using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace KanbanBoard.Presentation.Dialogs
{
    public class StartupDialogWindowViewModel : BindableBase
    {
        private string changelog, version;
        private readonly Action closeDialog;

        public StartupDialogWindowViewModel(Action closeDialog)
        {
            this.closeDialog = closeDialog;

            this.OkCommand = new DelegateCommand(this.CloseDialog);
            this.OpenWebpageCommand = new DelegateCommand<string>(this.OpenWebpage);

            this.Version = this.GetVersionString();
            this.Changelog = this.GetChangelog();
        }

        public string Version
        {
            get => this.version;
            set => this.SetProperty(ref this.version, value);
        }
        
        public string Changelog
        {
            get => this.changelog;
            set => this.SetProperty(ref this.changelog, value);
        }

        public ICommand OkCommand { get; }
        public ICommand OpenWebpageCommand { get; }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }

        private string GetVersionString()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return $"Stacket V {version}";
        }

        private string GetChangelog()
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://swegrock.github.io/stacket/changelog.txt");
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (WebException)
            {
                return string.Empty;
            }
        }

        private void OpenWebpage(string url)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}