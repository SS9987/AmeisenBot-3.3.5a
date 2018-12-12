using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotCombat.Interfaces
{
    public interface IAmeisenCombatPackage
    {
        List<Spell> Spells { get; }
        ISpellStrategy SpellStrategy { get; }
        IMovementStrategy MovementStrategy { get; }
    }
}
