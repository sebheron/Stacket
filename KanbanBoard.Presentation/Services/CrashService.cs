using KanbanBoard.Logic.Properties;
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
            var result = this.dialogService.ShowYesNo(Resources.Crash_Message, Resources.Stacket);
            if (!result.HasValue || !result.Value) return;
            var mail = Resources.Crash_Email_MailTo
                + string.Format(Resources.Crash_Email_Subject, Assembly.GetExecutingAssembly().GetName().Version)
                + string.Format(Resources.Crash_Email_Body, emailBody);
            Process.Start(new ProcessStartInfo("cmd", $"/c start {mail}") { CreateNoWindow = true });
        }
    }
}