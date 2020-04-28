using KanbanBoard.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace KanbanBoard.Tests {
   public static class BoardTestData {
      public static ColumnInformation GetRandomTestColumn(Random random) {
         int itemCount = random.Next(5);
         List<ItemInformation> items = new List<ItemInformation>();
         for (int i = 0; i < itemCount; i++) {
            ItemInformation item = GetRandomTestItem(random);
            items.Add(item);
         }
         return new ColumnInformation(Guid.NewGuid(), GetRandomNameOrDescription(random, 5), items);
      }

      public static ItemInformation GetRandomTestItem(Random random) {
         Array enumVals = Enum.GetValues(typeof(ItemTypes));
         string itemTitle = GetRandomNameOrDescription(random, 5);
         string itemDescription = GetRandomNameOrDescription(random, 10);
         ItemTypes itemType = (ItemTypes)enumVals.GetValue(random.Next(enumVals.Length));
         DateTime itemDueDate = new DateTime(random.Next(3000));
         Color itemColor = Color.FromArgb(255, (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));

         return new ItemInformation(Guid.NewGuid(), itemTitle, itemDescription, itemType, itemDueDate, itemColor);
      }

      public static BoardInformation GetRandomFileWithData(Random random) {
         string path = Path.GetTempFileName();
         int columnCount = random.Next(5);
         List<ColumnInformation> columns = new List<ColumnInformation>();
         for (int i = 0; i < columnCount; i++) {
            columns.Add(GetRandomTestColumn(random));
         }
         (new BoardInformation(path, columns)).Save();
         return new BoardInformation(path);
      }

      public static string GetRandomNameOrDescription(Random random, int length) {
         const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
         return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
      }
   }
}
