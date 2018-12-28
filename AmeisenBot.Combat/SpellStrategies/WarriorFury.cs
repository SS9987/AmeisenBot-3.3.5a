using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class WarriorFury : ICombatClass
    {
        private List<Spell> Spells { get; set; }

        private bool IsSlamKnown { get; set; }
        private bool IsBloodthirstKnown { get; set; }
        private bool IsWhirlwindKnown { get; set; }
        private bool IsBerserkerRageKnown { get; set; }
        public bool IsHeroicStrikeKnown { get; private set; }
        public bool IsHeroicThrowKnown { get; private set; }
        public bool IsExecuteKnown { get; private set; }

        private bool IsInMainCombo { get; set; }

        public WarriorFury(List<Spell> spells)
        {
            Spells = spells;

            IsSlamKnown = Spells.Where(spell => spell.Name == "Slam").ToList().Count > 0;
            IsBloodthirstKnown = Spells.Where(spell => spell.Name == "Bloodthirst").ToList().Count > 0;
            IsWhirlwindKnown = Spells.Where(spell => spell.Name == "Whirlwind").ToList().Count > 0;
            IsBerserkerRageKnown = Spells.Where(spell => spell.Name == "Berserker Rage").ToList().Count > 0;
            IsHeroicStrikeKnown = Spells.Where(spell => spell.Name == "Heroic Strike").ToList().Count > 0;
            IsHeroicThrowKnown = Spells.Where(spell => spell.Name == "Heroic Throw").ToList().Count > 0;
            IsExecuteKnown = Spells.Where(spell => spell.Name == "Execute").ToList().Count > 0;

            IsInMainCombo = false;
        }

        public Spell DoRoutine(Me me, Unit target)
        {
            List<string> MyAuras = AmeisenCore.GetAuras(LuaUnit.player);

            Spell spellToUse = null;
            if (Utils.GetDistance(me.pos, target.pos) < 3)
            {
                if (IsWhirlwindKnown && IsInMainCombo)
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Whirlwind").FirstOrDefault();
                    if (me.Energy < spellToUse.Costs)
                    {
                        return null;
                    }

                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        IsInMainCombo = false;
                        return spellToUse;
                    }
                }

                if (IsBerserkerRageKnown)
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Berserker Rage").FirstOrDefault();
                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        return spellToUse;
                    }
                }

                if (IsSlamKnown && MyAuras.Contains("slam!"))
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Slam").FirstOrDefault();
                    if (me.Energy < spellToUse.Costs)
                    {
                        return null;
                    }

                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        return spellToUse;
                    }
                }

                if (IsBloodthirstKnown)
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Bloodthirst").FirstOrDefault();
                    if (me.Energy < spellToUse.Costs)
                    {
                        return null;
                    }

                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        IsInMainCombo = true;
                        return spellToUse;
                    }
                }

                if (IsExecuteKnown)
                {
                    if (target.HealthPercentage < 15)
                    {
                        spellToUse = Spells.Where(spell => spell.Name == "Execute").FirstOrDefault();
                        if (me.Energy < spellToUse.Costs)
                        {
                            return null;
                        }

                        if (GetSpellCooldown(spellToUse.Name) < 0)
                        {
                            return spellToUse;
                        }
                    }
                }

                if (IsHeroicStrikeKnown)
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Heroic Strike").FirstOrDefault();
                    if (me.Energy < spellToUse.Costs)
                    {
                        return null;
                    }

                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        return spellToUse;
                    }
                }
            }

            if (Utils.GetDistance(me.pos, target.pos) < 30)
            {
                if (IsHeroicThrowKnown)
                {
                    spellToUse = Spells.Where(spell => spell.Name == "Heroic Throw").FirstOrDefault();
                    if (GetSpellCooldown(spellToUse.Name) < 0)
                    {
                        return spellToUse;
                    }
                }
            }
            return null;
        }

        private double GetSpellCooldown(string spellName)
        {
            double cooldown = double.Parse(AmeisenCore.GetLocalizedText($"start,duration,enabled = GetSpellCooldown(\"{spellName}\");cdLeft = (start + duration - GetTime()) * 1000;", "cdLeft"));
            return cooldown;
        }
    }
}
