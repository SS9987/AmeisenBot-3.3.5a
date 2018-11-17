using AmeisenBot.Character.Comparators;
using AmeisenBot.Character.Interfaces;
using AmeisenBot.Character.Objects;
using AmeisenBotCore;
using System;

namespace AmeisenBot.Character
{
    public class CharacterManager
    {
        public MeCharacter Character { get; private set; }

        public CharacterManager()
        {
            Character = new MeCharacter();
        }

        /// <summary>
        /// Update the whole character, may takes some time
        /// Updates stuff: Gear, Bags, Stats, Items
        /// </summary>
        public void UpdateCharacter() => Character.Update();

        /// <summary>
        /// This method equips better item if they are determined
        /// "better" by the supplied IItemComparator.
        ///
        /// Default Comparator is only looking for a better ItemLevel
        /// </summary>
        public void OnNewItemReceived(Item newItem, IItemComparator itemComparator = null)
        {
            if (itemComparator == null)
            {
                itemComparator = new BasicItemLevelComparator();
            }

            Item currentItem = GetCurrentItemByNewItem(newItem);
            if (itemComparator.Compare(newItem, currentItem))
            {
                AmeisenCore.LuaDoString($"EquipItemByName({newItem.Id}, {currentItem.Slot});");
                AmeisenCore.LuaDoString("ConfirmBindOnUse();");
            }
        }

        private Item GetCurrentItemByNewItem(Item newItem)
        {
            throw new NotImplementedException();
        }
    }
}