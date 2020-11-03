using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace KanbanBoard.Presentation.Services
{
    public class CrashService : ICrashService
    {
        private readonly IDialogService dialogService;

        public CrashService(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public void SendCrash(string emailBody)
        {
            if (dialogService.ShowYesNo("Stacket has encountered and error and has had to close. Should an error report be sent to the developers?", "Stacket"))
            {
                var subject = $"Stacket Bug Report - Version {Assembly.GetExecutingAssembly().GetName().Version}";
                Process.Start("mailto:stacketbugs@gmail.com?"
                    + $"subject={subject}"
                    + $"&body={emailBody}");
            }
        }
    }
}