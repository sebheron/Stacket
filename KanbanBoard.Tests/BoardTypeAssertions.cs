using KanbanBoard.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KanbanBoard.Tests
{
    public static class BoardTypeAssertions
    {
        public static void AssertSameBoard(BoardInformation boardInformation1, BoardInformation boardInformation2)
        {
            Assert.AreEqual(boardInformation1.FilePath, boardInformation2.FilePath);
            for (var i = 0; i < boardInformation1.ColumnCount; i++)
            {
                AssertColumn(boardInformation1.Columns[i], boardInformation1.Columns[i]);
            }
        }

        public static void AssertColumn(ColumnInformation columnInformation1, ColumnInformation columnInformation2)
        {
            Assert.AreEqual(columnInformation1.Items.Count, columnInformation2.Items.Count);
            for (var i = 0; i < columnInformation1.Items.Count; i++)
            {
                AssertItem(columnInformation1.Items[i], columnInformation2.Items[i]);
            }
        }

        public static void AssertItem(ItemInformation itemInformation1, ItemInformation itemInformation2)
        {
            Assert.AreEqual(itemInformation1.ItemTitle, itemInformation2.ItemTitle);
            Assert.AreEqual(itemInformation1.ItemDescription, itemInformation2.ItemDescription);
            Assert.AreEqual(itemInformation1.ItemType, itemInformation2.ItemType);
            Assert.AreEqual(itemInformation1.ItemDueDate, itemInformation2.ItemDueDate);
            Assert.AreEqual(itemInformation1.ItemColor, itemInformation2.ItemColor);
        }
    }
}