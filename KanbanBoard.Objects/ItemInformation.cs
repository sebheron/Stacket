using System;
using System.Text;
using System.Windows.Media;

namespace KanbanBoard.Objects {
   public class ItemInformation {
      public Guid ItemID { get; set; }
      public string ItemTitle { get; set; }
      public string ItemDescription { get; set; }
      public ItemTypes ItemType { get; set; }
      public DateTime ItemDueDate { get; set; }
      public Color ItemColor { get; set; }

      public ItemInformation(string itemTitle) {
         ItemID = Guid.NewGuid();
         ItemTitle = itemTitle;
         ItemDescription = string.Empty;
         ItemType = ItemTypes.Bug;
         ItemDueDate = DateTime.Now.Date;
         ItemColor = Color.FromArgb(255, 255, 0, 0);
      }

      public ItemInformation(Guid itemId, string itemTitle, string itemDescription, ItemTypes itemType, DateTime itemDueDate, Color itemColor) {
         ItemID = itemId;
         ItemTitle = itemTitle;
         ItemDescription = itemDescription;
         ItemType = itemType;
         ItemDueDate = itemDueDate.Date;
         ItemColor = itemColor;
      }

      public static ItemInformation Load(string parsedItem) {
         string[] itemData = parsedItem.Split(new string[] { "$%<NEWITEMDATA>%$" }, StringSplitOptions.None);
         Guid itemId = Guid.Parse(itemData[0]);
         string itemTitle = itemData[1];
         string itemDescription = itemData[2];
         ItemTypes itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), itemData[3]);
         DateTime itemDueDate = DateTime.Parse(itemData[4]).Date;
         Color itemColor = (Color)ColorConverter.ConvertFromString(itemData[5]);

         return new ItemInformation(itemId, itemTitle, itemDescription, itemType, itemDueDate, itemColor);
      }

      public override string ToString() {
         StringBuilder itemData = new StringBuilder();
         itemData.Append(ItemID);
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemTitle);
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemDescription);
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemType.ToString());
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemDueDate.ToString());
         itemData.Append("$%<NEWITEMDATA>%$");
         itemData.Append(ItemColor.ToString());
         return itemData.ToString();
      }
   }
}
