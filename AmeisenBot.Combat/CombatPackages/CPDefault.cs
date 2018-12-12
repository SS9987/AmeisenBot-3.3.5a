using AmeisenBotCombat.Interfaces;
using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using System.Collections.Generic;

namespace AmeisenBotCombat.CombatPackages
{
    public class CPDefault : IAmeisenCombatPackage
    {
        public List<Spell> Spells { get; private set; }
        public ISpellStrategy SpellStrategy { get; private set; }
        public IMovementStrategy MovementStrategy { get; private set; }

        public CPDefault(List<Spell> spells, ISpellStrategy spellStrategy, IMovementStrategy movementStrategy)
        {
            Spells = spells;
            SpellStrategy = spellStrategy;
            MovementStrategy = movementStrategy;
        }
    }
}
