using System;
using System.IO;

namespace KanbanBoard.DialogWindows
{
    public static class BoardHandling
    {
        public static string BoardFileStorageLocation => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Boards");
        public static string BoardFileExtension => ".brd";

        public static void Setup()
        {
            Directory.CreateDirectory(BoardFileStorageLocation);
        }
    }
}