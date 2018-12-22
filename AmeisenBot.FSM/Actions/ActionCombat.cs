using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenCombatEngineCore;
using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Objects;
using System;

namespace AmeisenBotFSM.Actions
{
    internal class ActionCombat : ActionMoving
    {
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private IAmeisenCombatPackage CombatPackage { get; set; }
        private CombatEngine CombatEngine { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private AmeisenBotUtilities.Unit Target
        {
            get { return AmeisenDataHolder.Target; }
            set { AmeisenDataHolder.Target = value; }
        }

        private AmeisenCombatEngineCore.Objects.Unit MeUnit
        {
            get
            {
                Me.Update();

                double energy = Me.Mana;
                double maxEnergy = Me.MaxMana;

                if (Me.Class == WowClass.Warrior)
                {
                    energy = Me.Rage;
                    maxEnergy = Me.MaxRage;
                }

                if (Me.Class == WowClass.Rogue)
                {
                    energy = Me.Energy;
                    maxEnergy = Me.MaxEnergy;
                }

                if (Me.Class == WowClass.DeathKnight)
                {
                    energy = Me.RuneEnergy;
                    maxEnergy = Me.MaxRuneEnergy;
                }

                return new AmeisenCombatEngineCore.Objects.Unit(
                    Me.Health,
                    Me.MaxHealth,
                    energy,
                    maxEnergy,
                    CombatState.Standing,
                    new AmeisenCombatEngineCore.Structs.Vector3(
                        Me.pos.X,
                        Me.pos.Y,
                        Me.pos.Z),
                    AmeisenCore.GetAuras(LuaUnit.player)
                    );
            }
        }


        private AmeisenCombatEngineCore.Objects.Unit TargetUnit
        {
            get
            {
                if (Target != null)
                {
                    Target.Update();

                    double energy = Target.Energy;
                    double maxEnergy = Target.MaxEnergy;

                    /*
                    if (Target.Class == WowClass.Warrior)
                    {
                        energy = Target.Rage;
                        maxEnergy = Target.MaxRage;
                    }

                    if (Target.Class == WowClass.Rogue)
                    {
                        energy = Target.Energy;
                        maxEnergy = Target.MaxEnergy;
                    }

                    if (Target.Class == WowClass.DeathKnight)
                    {
                        energy = Target.RuneEnergy;
                        maxEnergy = Target.MaxRuneEnergy;
                    }
                    */

                    return new AmeisenCombatEngineCore.Objects.Unit(
                        Target.Health,
                        Target.MaxHealth,
                        energy,
                        maxEnergy,
                        CombatState.Standing,
                        new AmeisenCombatEngineCore.Structs.Vector3(
                            Target.pos.X,
                            Target.pos.Y,
                            Target.pos.Z),
                        AmeisenCore.GetAuras(LuaUnit.target)
                        );
                }
                return new AmeisenCombatEngineCore.Objects.Unit(
                        0,
                        0,
                        0,
                        0,
                        CombatState.Standing,
                        new AmeisenCombatEngineCore.Structs.Vector3(
                            0,
                            0,
                            0),
                        AmeisenCore.GetAuras(LuaUnit.target)
                        );
            }
        }

        public ActionCombat(
            AmeisenDataHolder ameisenDataHolder,
            IAmeisenCombatPackage combatPackage,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            CombatPackage = combatPackage;
            CombatEngine = new CombatEngine(combatPackage.Spells, combatPackage.SpellStrategy, combatPackage.MovementStrategy);
            CombatEngine.OnCastSpell += HandleSpellCast;
            CombatEngine.OnMoveCharacter += HandleMovement;
        }

        private void HandleMovement(object sender, EventArgs e)
        {
            if (WaypointQueue.Count == 0)
            {
                Vector3 pos = new Vector3(
                ((MoveCharacterEventArgs)e).PositionToGoTo.X,
                ((MoveCharacterEventArgs)e).PositionToGoTo.Y,
                ((MoveCharacterEventArgs)e).PositionToGoTo.Z);

                Me.Update();

                if (!WaypointQueue.Contains(pos))
                {
                    WaypointQueue.Enqueue(pos);
                }
            }
        }

        public override void Start()
        {
            WaypointQueue.Clear();
            base.Start();
        }

        private void HandleSpellCast(object sender, EventArgs e)
        {
            CombatUtils.CastSpellByName(Me, Target, ((CastSpellEventArgs)e).Spell.SpellName, false, true);
            ((CastSpellEventArgs)e).Spell.StartCooldown();
        }

        public override void DoThings()
        {
            WaypointQueue.Clear();
            if (WaypointQueue.Count > 0)
            {
                base.DoThings();
            }

            if (AmeisenDataHolder.IsHealer)
            {
                CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);
                Target?.Update();
            }
            else
            {
                if (Target == null || Target.Guid == 0 || Target.Health == 0)
                {
                    CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
                }

                Target?.Update();

                if (Target == null || Target.Guid == 0)
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

            CombatEngine.DoIteration(MeUnit, TargetUnit);
        }
    }
}