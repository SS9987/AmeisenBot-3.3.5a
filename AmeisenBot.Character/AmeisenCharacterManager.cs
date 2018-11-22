using AmeisenBot.Character.Comparators;
using AmeisenBot.Character.Enums;
using AmeisenBot.Character.Interfaces;
using AmeisenBot.Character.Objects;
using AmeisenBotCore;
using AmeisenBotLogger;
using Newtonsoft.Json;
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

        /// <summary>
        /// This method equips better item if they are determined
        /// "better" by the supplied IItemComparator.
        ///
        /// Default Comparator is only looking for a better ItemLevel
        /// </summary>
        public void OnNewItemReceived(Item newItem, IItemComparator itemComparator = null)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"New Item received: {JsonConvert.SerializeObject(newItem)}", this);
            if (itemComparator == null)
            {
                itemComparator = new BasicItemLevelComparator();
            }
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Comapartor: {itemComparator.ToString()}", this);

            Item currentItem = GetCurrentItemByNewItem(newItem);
            if (currentItem == null || itemComparator.Compare(newItem, currentItem))
            {
                AmeisenCore.LuaDoString($"EquipItemByName({newItem.Id}, {currentItem.Slot});");
                AmeisenCore.LuaDoString("ConfirmBindOnUse();");
                AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
                AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Equipped new Item...", this);
            }
        }

        private Item GetCurrentItemByNewItem(Item newItem)
        {
            switch (newItem.Slot)
            {
                case (int)InventorySlot.INVSLOT_HEAD: return Character.Equipment.Head;
                case (int)InventorySlot.INVSLOT_NECK: return Character.Equipment.Necklace;
                case (int)InventorySlot.INVSLOT_BACK: return Character.Equipment.Back;
                case (int)InventorySlot.INVSLOT_SHOULDER: return Character.Equipment.Shoulder;
                case (int)InventorySlot.INVSLOT_CHEST: return Character.Equipment.Chest;
                case (int)InventorySlot.INVSLOT_TABARD: return Character.Equipment.Tabard;
                case (int)InventorySlot.INVSLOT_SHIRT: return Character.Equipment.Shirt;
                case (int)InventorySlot.INVSLOT_HANDS: return Character.Equipment.Hands;
                case (int)InventorySlot.INVSLOT_WRIST: return Character.Equipment.Wrist;
                case (int)InventorySlot.INVSLOT_WAIST: return Character.Equipment.Waist;
                case (int)InventorySlot.INVSLOT_LEGS: return Character.Equipment.Legs;
                case (int)InventorySlot.INVSLOT_FEET: return Character.Equipment.Feet;
                case (int)InventorySlot.INVSLOT_RING1: return Character.Equipment.RingOne;
                case (int)InventorySlot.INVSLOT_RING2: return Character.Equipment.RingTwo;
                case (int)InventorySlot.INVSLOT_TRINKET1: return Character.Equipment.TrinketOne;
                case (int)InventorySlot.INVSLOT_TRINKET2: return Character.Equipment.TrinketTwo;
                case (int)InventorySlot.INVSLOT_MAINHAND: return Character.Equipment.MainHand;
                case (int)InventorySlot.INVSLOT_OFFHAND: return Character.Equipment.OffHand;
                case (int)InventorySlot.INVSLOT_AMMO: return Character.Equipment.Ammo;
                case (int)InventorySlot.INVSLOT_RANGED: return Character.Equipment.Ranged;

                default:
                    return null;
            }
        }
    }
}