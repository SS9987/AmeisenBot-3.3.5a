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
                unitToAttack = CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
            }

            if (unitToAttack != null)
            {
                // Start autoattack
                if (!Me.InCombat)
                {
                    CombatUtils.FaceUnit(Me, unitToAttack);
                    CombatUtils.AttackTarget();
                }

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
            List<string> targetAuras = CombatUtils.GetAuras(LuaUnit.target);
            double distance = Utils.GetDistance(Me.pos, Target.pos);

            Me?.Update();
            Target?.Update();

            if (CombatUtils.IsSpellUseable("Charge"))
            {
                if (distance >= 10 && distance <= 23)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Charge", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Hamstring"))
            {
                if (Me.Mana >= 10 && !targetAuras.Contains("hamstring"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Hamstring", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Rend"))
            {
                if (Me.Mana >= 10 && !targetAuras.Contains("rend"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Rend", false);
                    return;
                }
            }


            if (CombatUtils.IsSpellUseable("Overpower"))
            {
                if (Me.Mana >= 5)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Overpower", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Mortal Strike"))
            {
                if (Me.Mana >= 30)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Mortal Strike", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Mocking Blow"))
            {
                if (Me.Mana >= 10)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Mocking Blow", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Heroic Strike"))
            {
                if (Me.Mana >= 12)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Heroic Strike", false);
                    return;
                }
            }
        }
    }
}