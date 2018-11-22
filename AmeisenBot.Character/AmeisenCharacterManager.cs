using AmeisenBot.Character.Comparators;
using AmeisenBot.Character.Interfaces;
using AmeisenBot.Character.Objects;
using AmeisenBotCore;
using AmeisenBotLogger;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBot.Character
{
    public class AmeisenCharacterManager
    {
        public MeCharacter Character { get; private set; }

        public AmeisenCharacterManager()
        {
            Character = new MeCharacter();
        }

        /// <summary>
        /// Update the whole character, may takes some time
        /// Updates stuff: Gear, Bags, Stats, Items
        /// </summary>
        public void UpdateCharacter() => new Thread(new ThreadStart(Character.Update)).Start();

        public void EquipAllBetterItems()
        {
            bool replacedItem = false;
            if (Character.FullyLoaded)
            {
                foreach (Item item in Character.Equipment.AsList())
                {
                    if (item.Id != 0)
                    {
                        List<InventoryItem> itemsLikeItem = GetAllItemsLike(item);

                        if (itemsLikeItem.Count > 0)
                        {
                            Item possibleNewItem = itemsLikeItem.First();
                            if (CompareItems(item, possibleNewItem))
                            {
                                ReplaceItem(item, possibleNewItem);
                                replacedItem = true;
                            }
                        }
                    }
                }
            }
            else { AmeisenLogger.Instance.Log(LogLevel.WARNING, "Could not Equip better items, Character is still loading", this); }

            if (replacedItem)
            {
                UpdateCharacter();
            }
        }

        private List<InventoryItem> GetAllItemsLike(Item item)
            => Character.InventoryItems.Where(s => s.EquipLocation == item.EquipLocation).OrderByDescending(x => x.Level).ToList();


        /// <summary>
        /// This method equips better item if they are determined
        /// "better" by the supplied IItemComparator.
        ///
        /// Default Comparator is only looking for a better ItemLevel
        /// </summary>
        public bool CompareItems(Item currentItem, Item newItem, IItemComparator itemComparator = null)
        {
            if (itemComparator == null)
            {
                itemComparator = new BasicItemLevelComparator();
            }
            if (currentItem == null || itemComparator.Compare(newItem, currentItem))
            {
                return true;
            }
            return false;
        }

        public void ReplaceItem(Item currentItem, Item newItem)
        {
            AmeisenCore.LuaDoString($"EquipItemByName(\"{newItem.Name}\", {currentItem.Slot});");
            AmeisenCore.LuaDoString("ConfirmBindOnUse();");
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Equipped new Item...", this);
        }
    }
}