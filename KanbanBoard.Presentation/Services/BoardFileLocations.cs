using System;
using System.IO;

namespace KanbanBoard.Presentation.Services
{
    public static class BoardFileLocations
    {
        public static string BoardFileStorageLocation => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Boards");
    }
}