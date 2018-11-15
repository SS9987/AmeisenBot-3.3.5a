using System;
using AmeisenBot.Character.Objects;

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
    }
}
