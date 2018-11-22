using AmeisenBot.Character.Enums;
using AmeisenBot.Character.Lua;
using AmeisenBot.Character.Structs;
using AmeisenBotCore;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using Newtonsoft.Json;
using System.Text;

namespace AmeisenBot.Character.Objects
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemLink { get; set; }
        public int Slot { get; private set; }
        public string Name { get; set; }
        public string ItemType { get; private set; }
        public string ItemSubtype { get; private set; }
        public int MaxStack { get; private set; }
        public int Count { get; set; }
        public int Level { get; set; }
        public int RequiredLevel { get; set; }
        public int Price { get; set; }
        public string EquipLocation { get; set; }

        public int Cooldown => CooldownEnd - CooldownStart;
        public int CooldownStart { get; set; }
        public int CooldownEnd { get; set; }

        public double Durability
        {
            get
            {
                if (DurabilityCurrent == 0 || DurabilityMax == 0)
                {
                    return -1;
                }
                else
                {
                    return DurabilityCurrent / DurabilityMax;
                }
            }
        }

        public int DurabilityMax { get; set; }
        public int DurabilityCurrent { get; set; }

        public ItemQuality Quality { get; set; }

        public PrimaryStats PrimaryStats { get; set; }

        public Item(int slot)
        {
            Slot = slot;
            Update();
        }

        private void Update()
        {
            // TODO: update item stuff here
            /*Id = ReadItemIntAttribute("GetInventoryItemID");
            Count = ReadItemIntAttribute("GetInventoryItemCount");
            Quality = (ItemQuality)ReadItemIntAttribute("GetInventoryItemQuality");
            (DurabilityMax, DurabilityCurrent) = ReadItemIntTuple("GetInventoryItemDurability");
            (CooldownStart, CooldownEnd) = ReadItemIntTuple("GetInventoryItemCooldown");

            Name = ReadItemDetail("itemName");
            Level = Utils.TryParseInt(ReadItemDetail("itemLevel"));
            RequiredLevel = Utils.TryParseInt(ReadItemDetail("itemMinLevel"));
            Price = Utils.TryParseInt(ReadItemDetail("itemSellPrice"));
            EquipLocation = ReadItemDetail("itemEquipLoc");*/

            // Experimental! but should be 100x faster
            string itemInfoJson = AmeisenCore.GetLocalizedText(GetItemInfo.Lua(Slot), GetItemInfo.OutVar());
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"GetItemInfoLuaJSON: {itemInfoJson}", this);
            // parse this JSON
            try
            {
                RawItem rawItem = (RawItem)JsonConvert.DeserializeObject(itemInfoJson);
                Id = int.Parse(rawItem.id);
                Count = int.Parse(rawItem.count);
                Quality = (ItemQuality)int.Parse(rawItem.quality);
                DurabilityCurrent = int.Parse(rawItem.curDurability);
                DurabilityMax = int.Parse(rawItem.maxDurability);
                CooldownStart = int.Parse(rawItem.cooldownStart);
                CooldownEnd = int.Parse(rawItem.cooldownEnd);
                Name = rawItem.name;
                ItemType = rawItem.type;
                ItemSubtype = rawItem.subtype;
                MaxStack = int.Parse(rawItem.maxStack);
                EquipLocation = rawItem.equiplocation;
                Price = int.Parse(rawItem.sellprice);
            }
            catch { }

            PrimaryStats = new PrimaryStats(this);
        }

        /// <summary>
        /// Read one of the following deatails:
        ///
        /// itemName, itemLink, itemRarity, itemLevel, itemMinLevel,
        /// itemType, itemSubType, itemStackCount, itemEquipLoc,
        /// itemIcon, itemSellPrice, itemClassID, itemSubClassID,
        /// bindType, expacID, itemSetID, isCraftingReagent.
        ///
        /// May returns nothing when the item is still beeing queried!
        /// </summary>
        /// <param name="detailToRead">detail to read</param>
        /// <returns>detail as string</returns>
        private string ReadItemDetail(string detailToRead)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("itemName, itemLink, itemRarity, itemLevel, itemMinLevel, ");
            cmd.Append("itemType, itemSubType, itemStackCount, itemEquipLoc, ");
            cmd.Append("itemIcon, itemSellPrice, itemClassID, itemSubClassID, ");
            cmd.Append("bindType, expacID, itemSetID, isCraftingReagent = ");
            cmd.Append($"GetItemInfo({Id})");
            return AmeisenCore.GetLocalizedText(cmd.ToString(), detailToRead);
        }

        /// <summary>
        /// Read an int tuple from item
        /// </summary>
        /// <param name="functionName">Function to run (example: GetInventoryItemDurability)</param>
        /// <returns>the int tuple you wanted to read</returns>
        private (int, int) ReadItemIntTuple(string functionName)
            => (Utils.TryParseInt(
                    AmeisenCore.GetLocalizedText(
                        $"xvara, xvarb = {functionName}(\"player\", {Slot});",
                        "xvara")
                ),
                Utils.TryParseInt(
                    AmeisenCore.GetLocalizedText(
                        $"xvara, xvarb = {functionName}(\"player\", {Slot});",
                        "xvarb")
                    )
                );

        /// <summary>
        /// Read a single int from an item
        /// </summary>
        /// <param name="functionName">Function to run (example: GetInventoryItemID)</param>
        /// <returns>the int you wanted to read</returns>
        private int ReadItemIntAttribute(string functionName)
            => Utils.TryParseInt(
                AmeisenCore.GetLocalizedText(
                    $"xvara = {functionName}(\"player\", {Slot});",
                    "xvara")
                );
    }
}