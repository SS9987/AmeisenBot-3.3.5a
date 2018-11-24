using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using System.Collections.Generic;

namespace AmeisenBotCombat.SampleClasses
{
    public class CCWarriorArms : IAmeisenCombatClass
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
            CombatUtils.CastSpellByName(Me, Target, "Battle Stance", false);
        }

        public void Exit()
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "CombatClass: Out of combat now", this);
        }

        public void HandleAttacking()
        {
            Me?.Update();
            Target?.Update();

            Unit unitToAttack = Target;

            // get a target
            if (Me.TargetGuid == 0 || Target.Health == 0)
            //|| !CombatUtils.IsHostile(LuaUnit.target)
            //|| !CombatUtils.CanAttack(LuaUnit.target))
            {
                unitToAttack = CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
            }

            Me?.Update();
            Target?.Update();

            if (Me.TargetGuid == 0 || Target.Health == 0)
            {
                CombatUtils.TargetNearestEnemy();
                Me.Update();
                Target.Update();
                unitToAttack = Target;
            }

            if (unitToAttack != null)
            {
                if (!CombatUtils.IsFacing(Me, unitToAttack))
                {
                    CombatUtils.FaceUnit(Me, unitToAttack);
                }

                if (!CombatUtils.IsInRange(Me, unitToAttack, 3.0))
                {
                    CombatUtils.MoveInRange(Me, unitToAttack, 2.0);
                }

                // start autoattack
                CombatUtils.AttackTarget();

                // only autoattacks
                DoAttackRoutine();
            }
        }

        public void HandleBuffs()
        {
            //List<string> myAuras = CombatUtils.GetAuras(LuaUnit.player);
        }

        public void HandleHealing()
        {
        }

        public void HandleTanking()
        {
        }

        private void DoAttackRoutine()
        {
            List<string> myAuras = CombatUtils.GetAuras(LuaUnit.player);
            List<string> targetAuras = CombatUtils.GetAuras(LuaUnit.target);
            double distance = Utils.GetDistance(Me.pos, Target.pos);

            Me?.Update();
            Target?.Update();

            if (CombatUtils.IsSpellUseable("Enraged Regeneration"))
            {
                if (Me.Rage >= 15 && Me.HealthPercentage <= 60)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Enraged Regeneration", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Retaliation"))
            {
                if (Me.HealthPercentage <= 40)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Retaliation", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Charge"))
            {
                if (distance >= 10 && distance <= 23)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Charge", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Herioc Throw"))
            {
                if (distance >= 10 && distance <= 28)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Herioc Throw", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Victory Rush"))
            {
                if (Me.HealthPercentage <= 80)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Victory Rush", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Battle Shout"))
            {
                if (Me.Rage >= 10 && !myAuras.Contains("battle shout"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Battle Shout", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Hamstring"))
            {
                if (Me.Rage >= 10 && !targetAuras.Contains("hamstring"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Hamstring", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Rend"))
            {
                if (Me.Rage >= 10 && !targetAuras.Contains("rend"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Rend", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Execute"))
            {
                if (Me.Rage >= 15)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Execute", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Overpower"))
            {
                if (Me.Rage >= 5)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Overpower", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Mortal Strike"))
            {
                if (Me.Rage >= 30)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Mortal Strike", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Mocking Blow"))
            {
                if (Me.Rage >= 10)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Mocking Blow", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Heroic Strike"))
            {
                if (Me.Rage >= 12)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Heroic Strike", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Slam"))
            {
                if (Me.Rage >= 15)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Slam", false);
                    return;
                }
            }
        }
    }
}