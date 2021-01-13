using System.Collections.Generic;

namespace KanbanBoard.Presentation.Services
{
    public interface IFTPService
    {
        IEnumerable<string> GetAllFiles();
        void GoOffline();
        void GoOnline();
        IEnumerable<string> ReadAllLines(string filePath);
        void WriteAllLines(string filePath, string[] boardData);
        void DeleteFile(string filePath);
        bool IsConnected { get; }
    }
}