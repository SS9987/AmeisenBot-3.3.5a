using AmeisenBot.Character.Objects;
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
        ICombatClass SpellStrategy { get; }
        IMovementStrategy MovementStrategy { get; }
    }
}
