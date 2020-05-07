using Prism.Mvvm;
using System;
using System.Text;
using System.Windows.Media;

namespace KanbanBoard.Objects
{
    public class ItemInformation : BindableBase
    {
        public Guid ItemID { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }
        public DateTime ItemDueDate { get; set; }

        private bool itemDescriptionVisible = false;
        public bool ItemDescriptionVisible {
            get => itemDescriptionVisible;
            set {
                SetProperty(ref itemDescriptionVisible, value);
            }
        }

        private Color itemColor;
        public Color ItemColor {
            get => itemColor;
            set {
                SetProperty(ref itemColor, value);
            }
        }

        private ItemTypes itemType;
        public ItemTypes ItemType {
            get => itemType;
            set {
                SetColor(value);
                SetProperty(ref itemType, value);
            }
        }

        private void SetColor(ItemTypes itemType) {
            switch (itemType) {
                case ItemTypes.Bug:
                    ItemColor = Color.FromArgb(255, 255, 159, 26);
                    break;
                case ItemTypes.Investigation:
                    ItemColor = Color.FromArgb(255, 64, 86, 161);
                    break;
                case ItemTypes.Item:
                    ItemColor = Color.FromArgb(255, 147, 158, 196);
                    break;
                case ItemTypes.Parked:
                    ItemColor = Color.FromArgb(255, 241, 60, 31);
                    break;
            }
        }

        public ItemInformation(string itemTitle) {
            ItemID = Guid.NewGuid();
            ItemTitle = itemTitle;
            ItemDescription = string.Empty;
            ItemType = ItemTypes.Item;
            ItemDueDate = DateTime.Now.Date;
            itemDescriptionVisible = true;
        }

        public ItemInformation(Guid itemId, string itemTitle, string itemDescription, ItemTypes itemType, DateTime itemDueDate, Color itemColor) {
            ItemID = itemId;
            ItemTitle = itemTitle;
            ItemDescription = itemDescription;
            ItemType = itemType;
            ItemDueDate = itemDueDate.Date;
            itemDescriptionVisible = true;
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

        public bool Unchanged() {
            bool sameTitle = ItemTitle == "New Item";
            bool sameDescription = ItemDescription == string.Empty;
            bool sameType = ItemType == ItemTypes.Item;
            return sameTitle & sameDescription & sameType;
        }
    }
}
