using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotUtilities;

namespace AmeisenBotFSM.Actions
{
    internal class ActionCombat : ActionMoving
    {
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private IAmeisenCombatPackage CombatPackage { get; set; }

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

        public ActionCombat(
            AmeisenDataHolder ameisenDataHolder,
            IAmeisenCombatPackage combatPackage,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            CombatPackage = combatPackage;
        }

        private void HandleMovement(Vector3 pos)
        {
            Me.Update();

            if (!WaypointQueue.Contains(pos))
            {
                WaypointQueue.Enqueue(pos);
            }
        }

        public override void Start()
        {
            WaypointQueue.Clear();
            base.Start();
        }

        public override void DoThings()
        {
            if (WaypointQueue.Count > 0)
            {
                base.DoThings();
                WaypointQueue.Clear();
            }

            if (AmeisenDataHolder.IsHealer)
            {
                CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);
                Target?.Update();
            }
            else
            {
                CombatUtils.AttackTarget();

                if (Target == null || Target.Guid == 0 || Target.Health == 0)
                {
                    CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
                }

                Target?.Update();

                if (Target == null || Target.Guid == 0 || CombatUtils.IsFriendly(LuaUnit.target))
                {
                    CombatUtils.TargetNearestEnemy();
                    Target?.Update();

                    if (Target == null || Target.Guid == 0)
                    {
                        return;
                    }
                }
            }

            if (!Me.InCombat && !AmeisenDataHolder.IsHealer)
            {
                CombatUtils.AttackTarget();
            }

            Spell spellToUse = CombatPackage.SpellStrategy.DoRoutine(Me, Target);
            if (spellToUse != null)
            {
                CombatUtils.CastSpellByName(Me, Target, spellToUse.Name, false, true);
            }

            Vector3 posToGoTo = CombatPackage.MovementStrategy.CalculatePosition(Me, Target);
            if (posToGoTo.X != Me.pos.X
                && posToGoTo.Y != Me.pos.Y
                && posToGoTo.Z != Me.pos.Z)
            {
                HandleMovement(posToGoTo);
            }
        }
    }
}