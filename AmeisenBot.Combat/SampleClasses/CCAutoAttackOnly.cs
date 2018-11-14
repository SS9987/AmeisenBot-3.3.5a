using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using System.Threading;

namespace AmeisenBotCombat.SampleClasses
{
    public class CCAutoAttackOnly : IAmeisenCombatClass
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
            Me?.Update();
            Target?.Update();

            Unit unitToAttack = Target;

            // get a target
            if (Me.TargetGuid == 0
                || !CombatUtils.IsHostile(LuaUnit.target)
                || !CombatUtils.CanAttack(LuaUnit.target))
            {
                unitToAttack = CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
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
                if (!Me.InCombat)
                {
                    CombatUtils.AttackTarget();
                }

                // only autoattacks
                DoAttackRoutine();
            }
        }

        public void HandleBuffs()
        {
        }

        public void HandleHealing()
        {
        }

        public void HandleTanking()
        {
        }

        private void DoAttackRoutine()
        {
            Thread.Sleep(1000);
        }
    }
}