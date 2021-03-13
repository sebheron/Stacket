using System;
using System.IO;

namespace KanbanBoard.Presentation.Services
{
    public static class FileLocations
    {
        public static string BoardFileStorageLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stacket Boards");
    }
}