using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using System.Collections.Generic;

namespace AmeisenBotCombat.SampleClasses
{
    public class CCHunterBeastmaster : IAmeisenCombatClass
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

        private Unit Pet
        {
            get { return AmeisenDataHolder.Pet; }
            set { AmeisenDataHolder.Pet = value; }
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

                if (!CombatUtils.IsInRange(Me, unitToAttack, 3.0))
                {
                    CombatUtils.MoveInRange(Me, unitToAttack, 2.0);
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

            if (Me.ManaPercentage > 30)
            {
                if (!myAuras.Contains("aspect of the hawk"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Aspect of the Hawk", true);
                }
            }
            else
            {
                if (!myAuras.Contains("aspect of the viper"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Aspect of the Viper", true);
                }
            }

            if (Me.PetGuid == 0)
            {
                CombatUtils.CastSpellByName(Me, Target, "Call Pet", true);
            }

            if (Pet != null && Pet.Guid != 0)
            {
                if (Pet.Health == 0)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Revive Pet", true);
                }

                if (Pet.HealthPercentage < 60)
                {
                    CombatUtils.CastSpellByName(Me, Target, "Mend Pet", true);
                }
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
            Target?.Update();

            if (Utils.GetDistance(Me.pos, Target.pos) < 15)
            {
                if (CombatUtils.IsSpellUseable("Frost Trap"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Frost Trap", false);
                    return;
                }
                if (CombatUtils.IsSpellUseable("Disengage"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Disengage", false);
                    return;
                }
            }

            if (!targetAuras.Contains("concussive shot"))
            {
                if (CombatUtils.IsSpellUseable("Concussive Shot"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Concussive Shot", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Viper Sting"))
            {
                CombatUtils.CastSpellByName(Me, Target, "Viper Sting", false);
                return;
            }

            if (CombatUtils.IsSpellUseable("Kill Command"))
            {
                CombatUtils.CastSpellByName(Me, Target, "Kill Command", false);
                return;
            }

            if (!targetAuras.Contains("serpent sting"))
            {
                if (CombatUtils.IsSpellUseable("Serpent Sting"))
                {
                    CombatUtils.CastSpellByName(Me, Target, "Serpent Sting", false);
                    return;
                }
            }

            if (CombatUtils.IsSpellUseable("Arcane Shot"))
            {
                CombatUtils.CastSpellByName(Me, Target, "Arcane Shot", false);
                return;
            }
        }
    }
}