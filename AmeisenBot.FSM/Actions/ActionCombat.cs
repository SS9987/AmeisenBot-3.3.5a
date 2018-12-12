using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotData;
using AmeisenBotFSM.Interfaces;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenCombatEngineCore;
using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.FSM.Enums;
using AmeisenCombatEngineCore.Objects;
using System;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    internal class ActionCombat : IAction
    {
        public Start StartAction { get { return Start; } }
        public DoThings StartDoThings { get { return DoThings; } }
        public Exit StartExit { get { return Stop; } }

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
                        Me.pos.Z)
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
                            Target.pos.Z)
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
                            0)
                        );
            }
        }

        public ActionCombat(AmeisenDataHolder ameisenDataHolder, IAmeisenCombatPackage combatPackage)
        {
            AmeisenDataHolder = ameisenDataHolder;
            CombatPackage = combatPackage;
            CombatEngine = new CombatEngine(MeUnit, TargetUnit, combatPackage.Spells, combatPackage.SpellStrategy, combatPackage.MovementStrategy);
            CombatEngine.OnCastSpell += HandleSpellCast;
        }

        private void HandleSpellCast(object sender, EventArgs e)
        {
            CombatUtils.CastSpellByName(Me, Target, ((CastSpellEventArgs)e).Spell.SpellName, false, true);
            ((CastSpellEventArgs)e).Spell.StartCooldown();
        }

        public void DoThings()
        {
            if (Target == null || Target.Guid == 0)
            {
                CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects);
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

            if (!Me.InCombat)
            {
                CombatUtils.AttackTarget();
            }

            CombatEngine.DoIteration(MeUnit, TargetUnit);
        }

        public void Start() { }

        public void Stop() { }
    }
}