using System;
using System.Windows.Media;
using KanbanBoard.Logic.Enums;
using KanbanBoard.Logic.Properties;
using Prism.Mvvm;

namespace KanbanBoard.Logic.BoardDataTypes
{
    public class ItemInformation : BindableBase
    {
        private const string NewItemData = "$%<NEWITEMDATA>%$";

        private string itemTitle;
        private Color itemColor;
        private bool itemDescriptionVisible;
        private ItemTypes itemType;

        public ItemInformation(string itemTitle)
        {
            this.ItemId = Guid.NewGuid();
            this.ItemTitle = itemTitle;
            this.ItemDescription = string.Empty;
            this.ItemType = Settings.Default.LastItemType;
            this.ItemDueDate = DateTime.Now.Date;
            this.ItemDescriptionVisible = false;
        }

        public ItemInformation(Guid itemId, string itemTitle, string itemDescription, ItemTypes itemType,
            DateTime itemDueDate)
        {
            this.ItemId = itemId;
            this.ItemTitle = itemTitle;
            this.ItemDescription = itemDescription;
            this.ItemType = itemType;
            this.ItemDueDate = itemDueDate.Date;
            this.ItemDescriptionVisible = false;
        }

        public Guid ItemId { get; set; }

        public string ItemTitle
        {
            get => itemTitle;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.SetProperty(ref itemTitle, value);
                }
            }
        }

        public string ItemDescription { get; set; }

        public DateTime ItemDueDate { get; set; }

        public bool ItemDescriptionVisible
        {
            get => this.itemDescriptionVisible;
            set => this.SetProperty(ref this.itemDescriptionVisible, value);
        }

        public Color ItemColor
        {
            get => this.itemColor;
            set => this.SetProperty(ref this.itemColor, value);
        }

        public ItemTypes ItemType
        {
            get => itemType;
            set
            {
                this.SetColor(value);
                this.SetProperty(ref this.itemType, value);
            }
        }

        public ItemTypes ItemTypeView
        {
            get => itemType;
            set
            {
                Settings.Default.LastItemType = value;
                Settings.Default.Save();
                this.SetColor(value);
                this.SetProperty(ref this.itemType, value);
            }
        }

        private void SetColor(ItemTypes item)
        {
            switch (item)
            {
                case ItemTypes.Bug:
                    this.ItemColor = Color.FromArgb(255, 255, 159, 26);
                    break;

                case ItemTypes.Investigation:
                    this.ItemColor = Color.FromArgb(255, 64, 86, 161);
                    break;

                case ItemTypes.Item:
                    this.ItemColor = Color.FromArgb(255, 147, 158, 196);
                    break;

                case ItemTypes.Parked:
                    this.ItemColor = Color.FromArgb(255, 241, 60, 31);
                    break;

                default:
                    this.ItemColor = Color.FromArgb(255, 147, 158, 196);
                    break;
            }
        }

        public static ItemInformation Load(string parsedItem)
        {
            var itemData = parsedItem.Split(new[] { NewItemData }, StringSplitOptions.None);
            var itemId = Guid.Parse(itemData[0]);
            var itemTitle = itemData[1];
            var itemDescription = itemData[2];
            var itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), itemData[3]);
            var itemDueDate = DateTime.Parse(itemData[4]).Date;

            return new ItemInformation(itemId, itemTitle, itemDescription, itemType, itemDueDate);
        }

        public override string ToString()
        {
            return this.ItemId + NewItemData + this.ItemTitle + NewItemData + this.ItemDescription
                   + NewItemData + this.ItemType + NewItemData + this.ItemDueDate + NewItemData + this.ItemColor;
        }

        public bool Unchanged()
        {
            return this.ItemTitle == "New Item"
                   && this.ItemDescription == string.Empty;
        }
    }
}