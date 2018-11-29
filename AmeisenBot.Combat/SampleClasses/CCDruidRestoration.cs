using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using System.Collections.Generic;

namespace AmeisenBotCombat.SampleClasses
{
    public class CCDruidRestoration : IAmeisenCombatClass
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
            if (Me.TargetGuid == 0)
            {
                unitToAttack = CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);
            }

            Target.Update();

            if (unitToAttack != null)
            {
                // Start autoattack
                if (!Me.InCombat)
                {
                    CombatUtils.FaceUnit(Me, unitToAttack);
                    //CombatUtils.AttackTarget(); // We are a heal class, no need to attack
                }

                //DoAttackRoutine();
            }
        }

        public void HandleBuffs()
        {
            LuaUnit[] unitsToCheckForBuff = { LuaUnit.player, LuaUnit.party1, LuaUnit.party2, LuaUnit.party3, LuaUnit.party4 };

            foreach (LuaUnit unit in unitsToCheckForBuff)
            {
                List<string> myAuras = CombatUtils.GetAuras(unit);
                if (!myAuras.Contains("mark of the wild"))
                {
                    CombatUtils.TargetLuaUnit(unit);
                    CombatUtils.CastSpellByName(Me, Target, "Mark of the Wild", true);
                    return;
                }
            }
        }

        public void HandleHealing()
        {
            if (Me != null)
            {
                Me.Update();
            }
            if (Target != null)
            {
                Target.Update();
            }

            Unit unitToAttack = CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);

            // Get a target
            if (Me.TargetGuid == 0)
            {
                CombatUtils.TargetLuaUnit(LuaUnit.player);
            }

            Me.Update();
            if (Target != null && Target.Guid != 0)
            {
                Target.Update();

                DoHealRoutine();
            }
        }

        public void HandleTanking()
        {
        }

        private void DoHealRoutine()
        {
            List<string> targetAuras = CombatUtils.GetAuras(LuaUnit.target);

            Me?.Update();
            Target?.Update();

            if (Target.HealthPercentage == 0)
            {
                return;
            }

            if (Target.HealthPercentage < 60)
            {
                if (!targetAuras.Contains("Healing Touch"))
                {
                    if (CombatUtils.IsInRange(Me, Target, 30))
                    {
                        CombatUtils.CastSpellByName(Me, Target, "Regrowth", false);
                        return;
                    }
                    else
                    {
                        CombatUtils.MoveInRange(Me, Target, 30);
                    }
                }
            }

            if (Target.HealthPercentage < 70)
            {
                if (!targetAuras.Contains("regrowth"))
                {
                    if (CombatUtils.IsInRange(Me, Target, 30))
                    {
                        CombatUtils.CastSpellByName(Me, Target, "Regrowth", false);
                        return;
                    }
                    else
                    {
                        CombatUtils.MoveInRange(Me, Target, 30);
                    }
                }
            }

            if (Target.HealthPercentage < 75)
            {
                if (!targetAuras.Contains("wild growth"))
                {
                    if (CombatUtils.IsInRange(Me, Target, 30))
                    {
                        CombatUtils.CastSpellByName(Me, Target, "Wild Growth", false);
                        return;
                    }
                    else
                    {
                        CombatUtils.MoveInRange(Me, Target, 30);
                    }
                }
            }

            if (Target.HealthPercentage < 80)
            {
                if (!targetAuras.Contains("rejuvenation"))
                {
                    if (CombatUtils.IsInRange(Me, Target, 30))
                    {
                        CombatUtils.CastSpellByName(Me, Target, "Rejuvenation", false);
                        return;
                    }
                    else
                    {
                        CombatUtils.MoveInRange(Me, Target, 30);
                    }
                }
            }

            if (Target.HealthPercentage < 90)
            {
                if (!targetAuras.Contains("lifebloom"))
                {
                    if (CombatUtils.IsInRange(Me, Target, 40))
                    {
                        CombatUtils.CastSpellByName(Me, Target, "Lifebloom", false);
                        return;
                    }
                    else
                    {
                        CombatUtils.MoveInRange(Me, Target, 40);
                    }
                }
            }
        }
    }
}