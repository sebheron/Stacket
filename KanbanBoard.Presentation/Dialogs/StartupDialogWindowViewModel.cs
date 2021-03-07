using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using KanbanBoard.Logic.Properties;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace KanbanBoard.Presentation.Dialogs
{
    public class StartupDialogWindowViewModel : BindableBase
    {
        private readonly Action closeDialog;

        public StartupDialogWindowViewModel(Action closeDialog)
        {
            this.closeDialog = closeDialog;

            this.OkCommand = new DelegateCommand(this.CloseDialog);
            this.OpenWebpageCommand = new DelegateCommand<string>(this.OpenWebpage);
        }

        public string Version => $"Stacket V{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        
        public string Changelog
        {
            get
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
        }

        public string Caption => Resources.Stacket;

        public ICommand OkCommand { get; }
        public ICommand OpenWebpageCommand { get; }

        private void CloseDialog()
        {
            this.closeDialog.Invoke();
        }

        private void OpenWebpage(string url)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}