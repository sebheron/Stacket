using FluentFTP;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KanbanBoard.Logic.Properties;
using System.IO;
using System.Net;
using Prism.Events;
using Kanban.Core.Events;
using System.Windows;
using System.IO.MemoryMappedFiles;
using System.Text;
using System;

namespace KanbanBoard.Presentation.Services
{
    public class FTPService : IFTPService
    {
        private readonly IEventAggregator eventAggregator;

        private FtpClient ftpClient;

        public FTPService(IEventAggregator eventAggregator)
        {
            this.ftpClient = this.CreateClient();
            if (Settings.Default.IsOnline)
            {
                Task.Run(ConnectToFTP);
            }
            this.eventAggregator = eventAggregator;
        }

        public bool IsConnected => this.ftpClient.IsConnected;

        private FtpClient CreateClient()
        {
            return new FtpClient
            {
                Host = "ftpupload.net",
                Credentials = new NetworkCredential("epiz_27657332", "qHSxdSlt24y71l"),
                UploadRateLimit = 0,
                DownloadRateLimit = 0
            };
        }

        public void GoOnline()
        {
            if (this.ftpClient.IsConnected) return;
            Task.Run(ConnectToFTP);
        }

        public void GoOffline()
        {
            if (!this.ftpClient.IsConnected) return;
            Task.Run(DisconnectFromFTP);
        }

        private async void ConnectToFTP()
        {
            await this.ftpClient.ConnectAsync();
            Application.Current.Dispatcher.Invoke(() => {
                Settings.Default.IsOnline = true;
                this.eventAggregator.GetEvent<FtpConnectionChangedEvent>().Publish(true);
            });
        }

        private async void DisconnectFromFTP()
        {
            await this.ftpClient.DisconnectAsync();
            Application.Current.Dispatcher.Invoke(() => {
                Settings.Default.IsOnline = false;
                this.eventAggregator.GetEvent<FtpConnectionChangedEvent>().Publish(false);
            });
        }

        public IEnumerable<string> GetAllFiles()
        {
            if (!this.ftpClient.IsConnected) yield break;
            var files = this.ftpClient.GetNameListing("/htdocs/htdocs");
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == Resources.BoardFileExtension)
                {
                    yield return file;
                }
            }
        }

        public IEnumerable<string> ReadAllLines(string filePath)
        {
            if (!this.ftpClient.IsConnected) yield break;
            filePath = "/htdocs/htdocs/" + filePath;
            if (this.ftpClient.FileExists(filePath))
            {
                using (var reader = new StreamReader(this.ftpClient.OpenRead(filePath)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        public void WriteAllLines(string filePath, string[] boardData)
        {
            if (!this.ftpClient.IsConnected) return;
            filePath = "/htdocs/htdocs/" + filePath;
            var bytes = Encoding.ASCII.GetBytes(boardData.Join("\n"));
            this.ftpClient.Upload(bytes, filePath);
        }

        public void DeleteFile(string filePath)
        {
            filePath = "/htdocs/htdocs/" + filePath;
            this.ftpClient.DeleteFile(filePath);
        }

        public DateTime GetFileModifiedDate(string filePath)
        {
            filePath = "/htdocs/htdocs/" + filePath;
            return ftpClient.GetModifiedTime(filePath);
        }
    }
}
