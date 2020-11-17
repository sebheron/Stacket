using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KanbanBoard.Logic.Enums;

namespace KanbanBoard.Tests
{
    public static class BoardTestData
    {
        /*public static ColumnInformation GetRandomTestColumn(Random random)
        {
            var itemCount = random.Next(5);
            var items = new List<ItemInformation>();
            for (var i = 0; i < itemCount; i++)
            {
                var item = GetRandomTestItem(random);
                items.Add(item);
            }

            return new ColumnInformation(Guid.NewGuid(), GetRandomNameOrDescription(random, 5), false, items);
        }

        public static ItemInformation GetRandomTestItem(Random random)
        {
            var enumVals = Enum.GetValues(typeof(ItemTypes));
            var itemTitle = GetRandomNameOrDescription(random, 5);
            var itemDescription = GetRandomNameOrDescription(random, 10);
            var itemType = (ItemTypes)enumVals.GetValue(random.Next(enumVals.Length));
            var itemDueDate = new DateTime(random.Next(3000));

            return new ItemInformation(Guid.NewGuid(), itemTitle, itemDescription, itemType, itemDueDate, false);
        }

        public static BoardInformation GetRandomFileWithData(Random random)
        {
            var path = Path.GetTempFileName();
            var columnCount = random.Next(5);
            var columns = new List<ColumnInformation>();
            for (var i = 0; i < columnCount; i++)
            {
                columns.Add(GetRandomTestColumn(random));
            }

            new BoardInformation(path, columns).Save();

            return new BoardInformation(path);
        }

        public static string GetRandomNameOrDescription(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }*/
    }
}