using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using System.Collections.Generic;

namespace AmeisenBotCombat.SampleClasses
{
    public class CCWarlockAffliction : IAmeisenCombatClass
    {
        public AmeisenDataHolder AmeisenDataHolder { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private Unit Target
        {
            get { return AmeisenDataHolder.Target; }
            set { AmeisenDataHolder.Target = value; }
        }

        public void Init()
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "CombatClass: In combat now", this);
        }

        public void Exit()
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "CombatClass: Out of combat now", this);
        }

        public void HandleAttacking()
        {
            if (Me != null)
            {
                Me.Update();
            }
            if (Target != null)
            {
                Target.Update();
            }

            Unit unitToAttack = Target;

            // Get a target
            if (Me.TargetGuid == 0 || CombatUtils.IsFriend(LuaUnit.target))
            {
                unitToAttack = CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
            }

            if (unitToAttack != null)
            {
                // Start autoattack
                if (!CombatUtils.IsFacingMelee(Me, unitToAttack))
                {
                    CombatUtils.FaceUnit(Me, unitToAttack);
                }

                if (!CombatUtils.IsInRange(Me, unitToAttack, 30.0))
                {
                    CombatUtils.MoveInRange(Me, unitToAttack, 27.0);
                }
                else
                {
                    CombatUtils.StopMovement(Me);
                }

                // start autoattack
                CombatUtils.AttackTarget();

                DoAttackRoutine();
            }
        }

        public void HandleBuffs()
        {
            List<string> myAuras = CombatUtils.GetAuras(LuaUnit.player);

            if (!myAuras.Contains("demon armor"))
            {
                CombatUtils.CastSpellByName(Me, Target, "Demon Armor", true);
            }
            if (!myAuras.Contains("blood pact"))
            {
                CombatUtils.CastSpellByName(Me, Target, "Summon Imp", true);
            }
        }

        public void HandleHealing()
        {
        }

        public void HandleTanking()
        {
        }

        private void DoAttackRoutine()
        {
            List<string> targetAuras = CombatUtils.GetAuras(LuaUnit.target);

            Me?.Update();
            // Restore Mana
            if (Me.ManaPercentage < 30 && Me.HealthPercentage > 50)
            {
                CombatUtils.CastSpellByName(Me, Target, "Life Tap", true);
                return;
            }

            if (Target.HealthPercentage < 5)
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Drain Soul", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            if (Me.HealthPercentage < 70)
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Death Coil", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            Target?.Update();
            // DoT's to apply
            if (!targetAuras.Contains("curse of agony"))
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Curse of Agony", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            if (!targetAuras.Contains("corruption"))
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Corruption", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            if (!targetAuras.Contains("unstable affliction"))
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Unstable Affliction", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            if (!targetAuras.Contains("haunt"))
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Haunt", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }

            Target?.Update();
            // Active-Damage Spell
            if (Target?.HealthPercentage < 25)
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Drain Soul", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }
            else
            {
                if (CombatUtils.IsInRange(Me, Target, 30))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Shadow Bolt", false);
                    return;
                }
                else
                {
                    CombatUtils.MoveInRange(Me, Target, 30);
                }
            }
        }
    }
}