using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
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

        private Unit Pet
        {
            get { return AmeisenDataHolder.Pet; }
            set { AmeisenDataHolder.Pet = value; }
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

        public override void Start()
        {
            // Startup Method
            if (CombatPackage.SpellStrategy != null)
            {
                // Updte me, target and pet
                Me?.Update();
                Target?.Update();
                Pet?.Update();

                CombatPackage.SpellStrategy.Startup(Me, Target, Pet);
            }
            WaypointQueue.Clear();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            WaypointQueue.Clear();
            AmeisenCore.RunSlashCommand("/cleartarget");
            Target = null;
        }

        public override void DoThings()
        {
            // Updte me, target and pet
            Me?.Update();
            Target?.Update();
            Pet?.Update();

            // Handle pending movement actions
            if (WaypointQueue.Count > 0)
            {
                base.DoThings();
            }

            // Try to get a target
            if (AmeisenDataHolder.IsHealer)
            {
                CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);
                Me?.Update();
                Target?.Update();
            }
            else
            {
                // clear all friendly targets
                AmeisenCore.RunSlashCommand("/cleartarget [noharm][dead]");

                if (Me.TargetGuid == 0 || Target == null || Target.Guid == 0)
                {
                    CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
                    // clear all friendly targets again
                    AmeisenCore.RunSlashCommand("/cleartarget [noharm][dead]");
                    Me?.Update();
                    Target?.Update();
                }


                if (Me.TargetGuid == 0 || Target == null || Target.Guid == 0)
                {
                    CombatUtils.TargetNearestEnemy();
                    Me?.Update();
                    Target?.Update();

                    if (Me.TargetGuid == 0 || Target == null || Target.Guid == 0)
                    {
                        // by now we should have a target
                        return;
                    }
                }
            }

            // Handle Movement stuff
            if (CombatPackage.MovementStrategy != null)
            {
                Me?.Update();
                Target?.Update();
                HandleMovement(CombatPackage.MovementStrategy.CalculatePosition(Me, Target));
            }

            // Attack target if we are no healer
            if (!Me.InCombat && !AmeisenDataHolder.IsHealer) { CombatUtils.AttackTarget(); }

            // Cast the Spell selected for this Iteration
            if (CombatPackage.SpellStrategy != null)
            {
                Me?.Update();
                Target?.Update();
                Spell spellToUse = CombatPackage.SpellStrategy.DoRoutine(Me, Target, Pet);
                if (spellToUse != null) { CombatUtils.CastSpellByName(Me, Target, spellToUse.Name, false, true); }
            }
        }

        private void HandleMovement(Vector3 pos)
        {
            Me.Update();

            if (!WaypointQueue.Contains(pos))
            {
                WaypointQueue.Enqueue(pos);
            }
        }
    }
}