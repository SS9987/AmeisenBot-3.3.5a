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

            public static List<Spell> Spells = new List<Spell>()
            {
                // Damage Casts
                new Spell("Frostbolt", 320, 34, 1000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1000 } }),
                new Spell("Frostfire Bolt", 400, 34, 1000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 950 } }),
                new Spell("Deep Freeze", 260, 34, 30000, SpellType.Damage, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 150 }, {SpellType.Stun, 5000 } }),
                new Spell("Ice Lance", 175, 34, 10000, SpellType.Damage, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 290 } }),
                new Spell("Cone of Cold", 720, 10, 8000, SpellType.Damage, SpellExecution.Melee, new Dictionary<SpellType, double>(){ { SpellType.Damage, 800 } }),
                new Spell("Mirror Image", 300, int.MaxValue, 180000, SpellType.Damage, SpellExecution.Melee, new Dictionary<SpellType, double>(){ { SpellType.Damage, 5000 } }),
                // Buffs
                new Spell("Ice Armor", 690, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                new Spell("Arcane Intellect", 1020, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                // Heals
                new Spell("Ice Barrier", 600, int.MaxValue, 24000, SpellType.Heal, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 500 } }),
                new Spell("Mana Shield", 200, 30, 1000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 1300 } }),
                // Stuns
                new Spell("Polymorph", 140, 30, 1000, SpellType.Stun, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Stun, 1500 } }),
                // Gapbuilder
                new Spell("Frost Nova", 200, 10, 20000, SpellType.Root, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 420 }, { SpellType.Root, 3000 } }),
            };
        }

        public abstract class Paladin
        {
            public static readonly int classID = 2;

            public static List<Spell> Spells = new List<Spell>()
            {
                // Damage Casts
                new Spell("Exorcism", 360, 30, 15000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1200 } }),
                new Spell("Judgement of Light", 220, 10, 10000, SpellType.Damage, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 150 } }),
                new Spell("Divine Storm", 480, 8, 10000, SpellType.Damage, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 600 } }),
                new Spell("Crusader Strike", 200, 10, 4000, SpellType.Damage, SpellExecution.Melee, new Dictionary<SpellType, double>(){ { SpellType.Damage, 500 } }),
                // Buffs
                new Spell("Retribution Aura", 0, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                new Spell("Seal of Wisdom", 620, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                new Spell("Blessing of Kings", 270, 30, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                // Heals
                new Spell("Flash of Light", 310, 38, 1000, SpellType.Heal, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 800 } }),
                new Spell("Holy Light", 1300, 30, 1000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 5000 } }),
                new Spell("Lay on Hands", 1300, 30, 1200000, SpellType.Heal, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 10000 } }),
                // Stuns
                new Spell("Hammer of Justice", 140, 10, 60000, SpellType.Stun, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Stun, 3000 } }),
            };
        }

        public abstract class Priest
        {
            public static readonly int classID = 5;

            public static List<Spell> Spells = new List<Spell>()
            {
                // Damage Casts
                new Spell("Smite", 580, 28, 1000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 800 } }),
                new Spell("Holy Fire", 430, 28, 10000, SpellType.Damage, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1200 } }),
                new Spell("Mind Blast", 660, 28, 8000, SpellType.Damage, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 900 } }),
                // Dots
                new Spell("Shadow Word: Pain", 770, 28, 1000, SpellType.Dot, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1450 } }),
                new Spell("Devouring Plague", 870, 28, 1000, SpellType.Dot, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Damage, 1400 } }),
                // Manarestore
                new Spell("Shadowfiend", 550, 28, 1500, SpellType.Manarestore, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Manarestore, 5000 } }),
                // Buffs
                new Spell("Divine Spirit", 905, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                new Spell("Power Word: Fortitude", 940, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                new Spell("Inner Fire", 490, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                // Hots
                new Spell("Prayer of Mending", 490, int.MaxValue, 1000, SpellType.Hot, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 1000 } }),
                new Spell("Renew", 490, int.MaxValue, 1000, SpellType.Hot, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 2000 } }),
                // Heals
                new Spell("Flash Heal", 700, 38, 1000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 2200 } }),
                new Spell("Greater Heal", 700, 38, 1000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 4600 } }),
                new Spell("Circle of Healing", 730, 38, 6000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 1500 } }),
                new Spell("Binding Heal", 1050, 38, 1000, SpellType.Heal, SpellExecution.Cast, new Dictionary<SpellType, double>(){ { SpellType.Heal, 1500 } }),
                // Fears
                new Spell("Psychic Scream", 530, 7, 27000, SpellType.Fear, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Fear, 3000 } }),
                // Shield
                new Spell("Power Word: Shield", 800, 38, 4000, SpellType.Shield, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Shield, 2600 } }),
            };
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
                // Buffs
                new Spell("Fel Armor", 1079, int.MaxValue, 1000, SpellType.Buff, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Damage, 0 } }),
                // Manarestores
                new Spell("Life Tap", 0, int.MaxValue, 1079, SpellType.Manarestore, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Manarestore, 2200 } }),
                new Spell("Dark Pact", 0, int.MaxValue, 1000, SpellType.Manarestore, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Manarestore, 300 } }),
                // Heals
                new Spell("Death Coil", 840, 30, 120000, SpellType.Heal, SpellExecution.Instant, new Dictionary<SpellType, double>(){ { SpellType.Heal, 2500 } }),
                new Spell("Drain Life", 0, 30, 5000, SpellType.Heal, SpellExecution.Channel, new Dictionary<SpellType, double>(){ { SpellType.Heal, 150 } }),
                // Fears
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