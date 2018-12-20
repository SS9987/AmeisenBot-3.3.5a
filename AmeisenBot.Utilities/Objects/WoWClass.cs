using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Objects;
using System.Collections.Generic;

namespace AmeisenBotUtilities
{
    public abstract class WoWClass
    {
        public abstract class DeathKnight
        {
            public static readonly int classID = 6;
        }

        public abstract class Druid
        {
            public static readonly int classID = 11;

            public enum ShapeshiftForms
            {
                BEAR,
                AQUA,
                CAT,
                TRAVEL,
                MOONKIN,
                TREE
            }
        }

        public abstract class Hunter
        {
            public static readonly int classID = 3;
        }

        public abstract class Mage
        {
            public static readonly int classID = 8;
        }

        public abstract class Paladin
        {
            public static readonly int classID = 2;
        }

        public abstract class Priest
        {
            public static readonly int classID = 5;
        }

        public abstract class Rogue
        {
            public static readonly int classID = 4;

            public enum ShapeshiftForms
            {
                STEALTH
            }
        }

        public abstract class Shaman
        {
            public static readonly int classID = 7;
        }

        public abstract class Warlock
        {
            public static readonly int classID = 9;

            public static List<Spell> Spells = new List<Spell>()
            {
                // Damage Casts
                new Spell("Shadow Bolt", 385, 30, 1000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 900 } }),
                // Dots
                new Spell("Corruption", 506, 30, 1000, SpellType.Dot, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1450 } }),
                new Spell("Haunt", 385, 30, 8000, SpellType.Dot, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 500 } }),
                new Spell("Unstable Affliction", 550, 30, 1500, SpellType.Dot, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 900 } }),
                // Debuffs
                new Spell("Curse of the Elements", 361, 30, 1000, SpellType.Debuff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                //Buffs
                new Spell("Fel Armor", 1079, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                // Manarestores
                new Spell("Life Tap", 0, int.MaxValue, 1079, SpellType.Manarestore, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Manarestore, 2200 } }),
                new Spell("Dark Pact", 0, int.MaxValue, 1000, SpellType.Manarestore, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Manarestore, 300 } }),
                // Heals
                new Spell("Death Coil", 840, 30, 120000, SpellType.Heal, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 2500 } }),
                new Spell("Drain Life", 0, 30, 5000, SpellType.Heal, SpellExecution.Channel, new Dictionary<SpellType, double>(){ { SpellType.Heal, 150 } }),
                // Heals
                new Spell("Fear", 434, 30, 1000, SpellType.Fear, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Fear, 2000 } }),

                //new Spell("Summon Imp", 2500, int.MaxValue, 10000, SpellType.Buff, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
            };
        }

        public abstract class Warrior
        {
            public static readonly int classID = 1;

            public enum ShapeshiftForms
            {
                BATTLE,
                DEFENSIVE,
                BERSERKER,
            }
        }
    }
}