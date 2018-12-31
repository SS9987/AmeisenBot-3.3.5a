using AmeisenBot.Character.Objects;
using AmeisenBotData;
using AmeisenBotUtilities;

namespace AmeisenBotCombat.Interfaces
{
    public interface ICombatClass
    {
        Spell DoRoutine(Me me, Unit target, Unit pet);
        void Startup(Me me, Unit target, Unit pet);
    }
}