using AmeisenBot.Character;
using AmeisenBotCombat;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotFSM.Enums;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenCombatEngine.Interfaces;
using AmeisenMovement;
using System.Threading;

namespace AmeisenBotFSM
{
    public class AmeisenStateMachineManager
    {
        public bool Active { get; private set; }
        public bool PushedCombat { get; private set; }
        public AmeisenStateMachine StateMachine { get; private set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private IAmeisenCombatClass CombatClass { get; set; }
        private Thread MainWorker { get; set; }
        private Thread StateWatcherWorker { get; set; }

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

        public AmeisenStateMachineManager(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenMovementEngine ameisenMovementEngine,
            IAmeisenCombatClass combatClass,
            AmeisenCharacterManager characterManager)
        {
            Active = false;

            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            CombatClass = combatClass;

            MainWorker = new Thread(new ThreadStart(DoWork));
            StateWatcherWorker = new Thread(new ThreadStart(WatchForStateChanges));
            StateMachine = new AmeisenStateMachine(ameisenDataHolder, ameisenDBManager, ameisenMovementEngine, combatClass, characterManager);
        }

        /// <summary>
        /// Fire up the FSM
        /// </summary>
        public void Start()
        {
            if (!Active)
            {
                Active = true;
                MainWorker.Start();
                StateWatcherWorker.Start();
            }
        }

        /// <summary>
        /// Shutdown the FSM
        /// </summary>
        public void Stop()
        {
            if (Active)
            {
                Active = false;
                MainWorker.Join();
                StateWatcherWorker.Join();
            }
        }

        /// <summary>
        /// Update the Statemachine, let it do its work
        /// </summary>
        private void DoWork()
        {
            while (Active)
            {
                // Do the Actions
                StateMachine.Update();
                Thread.Sleep(AmeisenDataHolder.Settings.stateMachineUpdateMillis);
            }
        }

        /// <summary>
        /// Change the state of out FSM
        /// </summary>
        private void WatchForStateChanges()
        {
            while (Active)
            {
                Thread.Sleep(AmeisenDataHolder.Settings.stateMachineStateUpdateMillis);

                // Am I in combat
                if (InCombatCheck())
                {
                    continue;
                }

                // Do i need to heal
                if (AmeisenDataHolder.IsAllowedToHeal)
                {
                    CombatClass.HandleHealing();
                }

                // Do i need to buff
                if (AmeisenDataHolder.IsAllowedToBuff)
                {
                    CombatClass?.HandleBuffs();
                }

                // Am I dead?
                if (DeadCheck())
                {
                    continue;
                }

                // Bot stuff check
                if (BotStuffCheck())
                {
                    continue;
                }

                // Is me supposed to follow
                if (FollowCheck())
                {
                    continue;
                }

                // Do I need to release my spirit
                if (ReleaseSpiritCheck())
                {
                    continue;
                }

                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"FSM: {StateMachine.GetCurrentState()}", this);
            }
        }

        private bool BotStuffCheck()
        {
            if (AmeisenDataHolder.IsAllowedToDoOwnStuff)
            {
                if (StateMachine.GetCurrentState() == BotState.Idle)
                {
                    StateMachine.PushAction(BotState.BotStuff);
                }
                return true;
            }
            else if (StateMachine.GetCurrentState() == BotState.BotStuff)
            {
                StateMachine.PopAction();
            }
            return false;
        }

        private bool FollowCheck()
        {
            if (Me.PartyleaderGUID != 0)
            {
                Unit activeUnit = null;
                foreach (WowObject p in AmeisenDataHolder.ActiveWoWObjects)
                {
                    if (p.Guid == Me.PartyleaderGUID)
                    {
                        activeUnit = (Unit)p;
                    }
                }

                Me.Update();
                activeUnit?.Update();
                if (activeUnit != null)
                {
                    double distance = Utils.GetDistance(Me.pos, activeUnit.pos);

                    if (AmeisenDataHolder.IsAllowedToFollowParty && distance > AmeisenDataHolder.Settings.followDistance)
                    {
                        if (StateMachine.GetCurrentState() == BotState.Idle)
                        {
                            StateMachine.PushAction(BotState.Follow);
                        }
                        return true;
                    }
                    else if (StateMachine.GetCurrentState() == BotState.Follow)
                    {
                        StateMachine.PopAction();
                    }
                }
            }
            return false;
        }

        private bool InCombatCheck()
        {
            if (Me != null)
            {
                if (Me.InCombat
                    || (AmeisenDataHolder.IsAllowedToAssistParty
                    && CombatUtils.GetPartymembersInCombat(Me, AmeisenDataHolder.ActiveWoWObjects).Count > 0))
                {
                    if (StateMachine.GetCurrentState() != BotState.Idle)
                    {
                        StateMachine.PopAction();
                    }

                    if (StateMachine.GetCurrentState() != BotState.Combat)
                    {
                        StateMachine.PushAction(BotState.Combat);
                    }
                    return true;
                }
                else if (StateMachine.GetCurrentState() == BotState.Combat)
                {
                    StateMachine.PopAction();
                }
            }
            return false;
        }

        private bool ReleaseSpiritCheck()
        {
            if (AmeisenDataHolder.IsAllowedToReleaseSpirit)
            {
                if (Me.Health == 0)
                {
                    AmeisenCore.ReleaseSpirit();
                    Thread.Sleep(2000);
                    return true;
                }
            }
            return false;
        }

        private bool DeadCheck()
        {
            if (AmeisenDataHolder.IsAllowedToRevive)
            {
                if (Me.IsDead)
                {
                    if (StateMachine.GetCurrentState() != BotState.Idle)
                    {
                        StateMachine.PopAction();
                    }

                    StateMachine.PushAction(BotState.Dead);
                    return true;
                }
                else if (StateMachine.GetCurrentState() == BotState.Dead)
                {
                    StateMachine.PopAction();
                }
            }
            return false;
        }
    }
}