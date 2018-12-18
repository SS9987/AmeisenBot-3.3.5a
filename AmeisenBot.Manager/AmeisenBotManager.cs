using AmeisenBot.Character;
using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotCombat.CombatPackages;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotFSM;
using AmeisenBotFSM.Enums;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Objects;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Strategies;
using AmeisenMovement;
using AmeisenMovement.Formations;
using Magic;
using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using Unit = AmeisenBotUtilities.Unit;

namespace AmeisenBotManager
{
    /// <summary>
    /// This Singleton provides an Interface to the bot at a single point
    /// </summary>
    public class BotManager
    {
        private readonly string sqlConnectionString =
                "server={0};port={1};database={2};uid={3};password={4};";

        public AmeisenDBManager AmeisenDBManager { get; private set; }
        public List<WowObject> ActiveWoWObjects { get { return AmeisenDataHolder.ActiveWoWObjects; } }

        public bool IsAllowedToAssistParty
        {
            get { return AmeisenDataHolder.IsAllowedToAssistParty; }
            set { AmeisenDataHolder.IsAllowedToAssistParty = value; }
        }

        public bool IsAllowedToDoRandomEmotes
        {
            get { return AmeisenDataHolder.IsAllowedToDoRandomEmotes; }
            set { AmeisenDataHolder.IsAllowedToDoRandomEmotes = value; }
        }

        public bool IsAllowedToAttack
        {
            get { return AmeisenDataHolder.IsAllowedToAttack; }
            set { AmeisenDataHolder.IsAllowedToAttack = value; }
        }

        public bool IsAllowedToDoOwnStuff
        {
            get { return AmeisenDataHolder.IsAllowedToDoOwnStuff; }
            set { AmeisenDataHolder.IsAllowedToDoOwnStuff = value; }
        }

        public bool IsAllowedToBuff
        {
            get { return AmeisenDataHolder.IsAllowedToBuff; }
            set { AmeisenDataHolder.IsAllowedToBuff = value; }
        }

        public bool IsAllowedToFollowParty
        {
            get { return AmeisenDataHolder.IsAllowedToFollowParty; }
            set { AmeisenDataHolder.IsAllowedToFollowParty = value; }
        }

        public bool IsAllowedToHeal
        {
            get { return AmeisenDataHolder.IsAllowedToHeal; }
            set { AmeisenDataHolder.IsAllowedToHeal = value; }
        }

        public bool IsAllowedToTank
        {
            get { return AmeisenDataHolder.IsAllowedToTank; }
            set { AmeisenDataHolder.IsAllowedToTank = value; }
        }

        public bool IsAllowedToReleaseSpirit
        {
            get { return AmeisenDataHolder.IsAllowedToReleaseSpirit; }
            set { AmeisenDataHolder.IsAllowedToReleaseSpirit = value; }
        }

        public bool IsAllowedToRevive
        {
            get { return AmeisenDataHolder.IsAllowedToRevive; }
            set { AmeisenDataHolder.IsAllowedToRevive = value; }
        }

        public bool IsConnectedToDB
        {
            get { return AmeisenDataHolder.IsConnectedToDB; }
            set { AmeisenDataHolder.IsConnectedToDB = value; }
        }

        public bool IsConnectedToServer
        {
            get { return AmeisenDataHolder.IsConnectedToServer; }
            set { AmeisenDataHolder.IsConnectedToServer = value; }
        }

        public bool IsBlackmagicAttached { get; private set; }
        public bool IsEndsceneHooked { get; private set; }
        public Me Me { get { return AmeisenDataHolder.Me; } }
        public List<WowExe> RunningWows { get { return AmeisenCore.GetRunningWows(); } }
        public Settings Settings { get { return AmeisenSettings.Settings; } }
        public Unit Target { get { return AmeisenDataHolder.Target; } }
        public Unit Pet { get { return AmeisenDataHolder.Pet; } }
        public WowExe WowExe { get; private set; }
        public Process WowProcess { get; private set; }
        public MeCharacter Character => AmeisenCharacterManager.Character;
        public int MapID { get { return AmeisenCore.GetMapID(); } }
        public int ZoneID { get { return AmeisenCore.GetZoneID(); } }
        public string LoadedConfigName { get { return AmeisenSettings.loadedconfName; } }
        public int HookJobsInQueue => AmeisenHook.JobCount;
        /// <summary>
        /// Returns copper, silver, gold as an array
        /// </summary>
        public int[] Money
        {
            get
            {
                int copper = AmeisenCharacterManager.Character.Money % 100;
                int silver = (AmeisenCharacterManager.Character.Money - copper) % 10000 / 100;
                int gold = (AmeisenCharacterManager.Character.Money - (silver * 100 + copper)) / 10000;
                return new int[] { copper, silver, gold };
            }
        }

        public List<NetworkBot> NetworkBots
        {
            get
            {
                if (AmeisenClient.IsRegistered)
                {
                    return AmeisenClient.Bots;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsIngame
        {
            get
            {
                return AmeisenCore.CheckWorldLoaded()
                   && !AmeisenCore.CheckLoadingScreen(); // TODO: implement this
            }
        }

        public bool IsRegisteredAtServer { get { return AmeisenClient.IsRegistered; } }
        public BotState CurrentFSMState { get { return AmeisenStateMachineManager.StateMachine.GetCurrentState(); } }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenClient AmeisenClient { get; set; }
        private AmeisenNavmeshClient AmeisenNavmeshClient { get; set; }
        private AmeisenHook AmeisenHook { get; set; }
        private AmeisenObjectManager AmeisenObjectManager { get; set; }
        private AmeisenSettings AmeisenSettings { get; set; }
        private AmeisenStateMachineManager AmeisenStateMachineManager { get; set; }
        private AmeisenMovementEngine AmeisenMovementEngine { get; set; }
        private BlackMagic Blackmagic { get; set; }
        private AmeisenEventHook AmeisenEventHook { get; set; }
        private AmeisenCharacterManager AmeisenCharacterManager { get; set; }
        public string CurrentCombatClass { get; set; }
        private Queue<Unit> LootableUnits
        {
            get => AmeisenDataHolder.LootableUnits;
            set => AmeisenDataHolder.LootableUnits = value;
        }

        /// <summary>
        /// Create a new AmeisenBotManager to manage the bot's functionality
        /// </summary>
        public BotManager()
        {
            IsBlackmagicAttached = false;
            IsEndsceneHooked = false;

            AmeisenDataHolder = new AmeisenDataHolder();
            AmeisenSettings = new AmeisenSettings(AmeisenDataHolder);
            AmeisenClient = new AmeisenClient(AmeisenDataHolder);
            AmeisenDBManager = new AmeisenDBManager(AmeisenDataHolder);
        }

        /// <summary>
        /// Load a given CombatClass *.cs file into the CombatManager by compiling it at runtime
        /// </summary>
        /// <param name="fileName">*.cs CombatClass file</param>
        public void LoadCombatClassFromFile(string fileName)
        {
            AmeisenSettings.Settings.combatClassPath = fileName;
            AmeisenSettings.SaveToFile(AmeisenSettings.loadedconfName);
            IAmeisenCombatPackage combatClass = CompileAndLoadCombatClass(fileName);
            AmeisenStateMachineManager.StateMachine.LoadNewCombatClass(AmeisenDataHolder, combatClass);
            CurrentCombatClass = combatClass.ToString();
        }

        /// <summary>
        /// Loads the Settings from a given file
        /// </summary>
        /// <param name="filename">file to load the Settings from</param>
        public void LoadSettingsFromFile(string filename) => AmeisenSettings.LoadFromFile(filename);

        /// <summary>
        /// Save the current Settings to the given file
        /// </summary>
        /// <param name="filename">file to save the Settings to</param>
        public void SaveSettingsToFile(string filename) => AmeisenSettings.SaveToFile(filename);

        /// <summary>
        /// Starts the bots mechanisms, hooks, ...
        /// </summary>
        /// <param name="wowExe">WowExe to start the bot on</param>
        public void StartBot(WowExe wowExe)
        {
            WowExe = wowExe;
            LootableUnits = new Queue<Unit>();

            // Load Settings
            AmeisenSettings.LoadFromFile(wowExe.characterName);

            // Load old WoW Position
            if (AmeisenSettings.Settings.saveBotWindowPosition)
            {
                if (AmeisenSettings.Settings.wowRectL >= 0
                && AmeisenSettings.Settings.wowRectR >= 0
                && AmeisenSettings.Settings.wowRectT >= 0
                && AmeisenSettings.Settings.wowRectB >= 0)
                {
                    AmeisenCore.SetWindowPosition(
                    wowExe.process.MainWindowHandle,
                    (int)AmeisenSettings.Settings.wowRectL,
                    (int)AmeisenSettings.Settings.wowRectT,
                    (int)AmeisenSettings.Settings.wowRectB - (int)AmeisenSettings.Settings.wowRectT,
                    (int)AmeisenSettings.Settings.wowRectR - (int)AmeisenSettings.Settings.wowRectL);
                }
            }

            // Connect to DB
            if (AmeisenSettings.Settings.databaseAutoConnect)
            {
                ConnectToDB();
            }

            // Connect to NavmeshServer
            if (AmeisenSettings.Settings.navigationServerAutoConnect)
            {
                AmeisenNavmeshClient = new AmeisenNavmeshClient(
                    AmeisenSettings.Settings.navigationServerIp,
                    AmeisenSettings.Settings.navigationServerPort
                );
            }

            // Attach to Proccess
            Blackmagic = new BlackMagic(wowExe.process.Id);
            IsBlackmagicAttached = Blackmagic.IsProcessOpen;
            // TODO: make this non static
            AmeisenCore.BlackMagic = Blackmagic;

            // Hook EndScene
            AmeisenHook = new AmeisenHook(Blackmagic);
            IsEndsceneHooked = AmeisenHook.isHooked;
            // TODO: make this non static
            AmeisenCore.AmeisenHook = AmeisenHook;

            // Hook Events
            AmeisenEventHook = new AmeisenEventHook();
            AmeisenEventHook.Init();
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_ENTERING_WORLD, OnPlayerEnteringWorld);
            AmeisenEventHook.Subscribe(WowEvents.LOOT_OPENED, OnLootWindowOpened);
            AmeisenEventHook.Subscribe(WowEvents.LOOT_BIND_CONFIRM, OnLootBindOnPickup);
            AmeisenEventHook.Subscribe(WowEvents.READY_CHECK, OnReadyCheck);
            AmeisenEventHook.Subscribe(WowEvents.PARTY_INVITE_REQUEST, OnPartyInvitation);
            AmeisenEventHook.Subscribe(WowEvents.CONFIRM_SUMMON, OnSummonRequest);
            AmeisenEventHook.Subscribe(WowEvents.RESURRECT_REQUEST, OnResurrectRequest);
            AmeisenEventHook.Subscribe(WowEvents.ITEM_PUSH, OnNewItem);
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_REGEN_DISABLED, OnRegenDisabled);
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_REGEN_ENABLED, OnRegenEnabled);


            // Start our object updates
            AmeisenObjectManager = new AmeisenObjectManager(AmeisenDataHolder, AmeisenDBManager);
            AmeisenObjectManager.Start();

            // Load the combatclass
            IAmeisenCombatPackage combatClass = CompileAndLoadCombatClass(AmeisenSettings.Settings.combatClassPath);
            if (combatClass == null)
            {
                combatClass = LoadDefaultClassForSpec();
            }

            // Init our MovementEngine to position ourself according to our formation
            AmeisenMovementEngine = new AmeisenMovementEngine(new DefaultFormation())
            {
                MemberCount = 40
            };

            // Start the StateMachine
            AmeisenStateMachineManager = new AmeisenStateMachineManager(
                AmeisenDataHolder,
                AmeisenDBManager,
                AmeisenMovementEngine,
                combatClass,
                AmeisenCharacterManager,
                AmeisenNavmeshClient);

            // Deafult Idle state
            AmeisenStateMachineManager.StateMachine.PushAction(BotState.Idle);
            AmeisenStateMachineManager.Start();

            // Init our CharacterMangager to keep track of our stats/items/money
            AmeisenCharacterManager = new AmeisenCharacterManager();
            AmeisenCharacterManager.UpdateCharacterAsync();

            // Connect to Server
            if (Settings.serverAutoConnect)
            {
                ConnectToServer();
            }
        }

        private IAmeisenCombatPackage LoadDefaultClassForSpec()
        {
            List<Spell> Spells = new List<Spell>();

            switch (Me.Class)
            {
                case WowClass.Warlock:
                    return new CPDefault(
                        WoWClass.Warlock.Spells,
                        new SpellSimple(WoWClass.Warlock.Spells),
                        new MovementCloseCombat()
                    );

                default:
                    return new CPDefault(
                        Spells, new SpellSimple(Spells), new MovementCloseCombat());
            }
        }

        private void OnRegenEnabled(long timestamp, List<string> args)
        {
            Me.InCombatEvent = false;
            CheckForLoot();
        }

        private void CheckForLoot()
        {
            foreach (WowObject obj in ActiveWoWObjects)
            {
                if (obj.GetType() == typeof(Player)
                    || obj.GetType() == typeof(Unit)
                    || obj.GetType() == typeof(Me))
                {
                    if (!((Unit)obj).IsDead)
                    {
                        continue; // We cant loot alive targets lel
                    }

                    obj.Update();
                    if (((Unit)obj).IsLootable)
                    {
                        LootableUnits.Enqueue((Unit)obj);
                    }
                    continue;
                }
            }
        }

        private void OnRegenDisabled(long timestamp, List<string> args)
            => Me.InCombatEvent = true;

        private void OnNewItem(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnNewItem args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCharacterManager.UpdateCharacter();
            AmeisenCharacterManager.EquipAllBetterItems();
        }

        /// <summary>
        /// Equip all better items thats in the bots inventory
        /// </summary>
        public void EquipAllBetterItems() => AmeisenCharacterManager.EquipAllBetterItems();

        /// <summary>
        /// Refresh the characters equipment & inventory
        /// </summary>
        public void RefreshCurrentItems() => AmeisenCharacterManager.UpdateCharacterAsync();

        private void OnResurrectRequest(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnResurrectRequest args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptResurrect();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }

        private void OnSummonRequest(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnSummonRequest args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptSummon();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }

        private void OnPartyInvitation(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnPartyInvitation args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptGroupInvite();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }

        private void OnReadyCheck(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnReadyCheck args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.ConfirmReadyCheck();
        }

        private void OnLootBindOnPickup(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnLootBindOnPickup args: {JsonConvert.SerializeObject(args)}",
                this
            );
        }

        private void OnLootWindowOpened(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnLootWindowOpened args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.LootEveryThing();
        }

        private void OnPlayerEnteringWorld(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"[{timestamp}] OnPlayerEnteringWorld args: {JsonConvert.SerializeObject(args)}",
                this
            );
        }

        private bool ConnectToServer() => AmeisenClient.Register(
                Me,
                IPAddress.Parse(AmeisenSettings.Settings.ameisenServerIp),
                AmeisenSettings.Settings.ameisenServerPort
            );

        private bool ConnectToDB() => AmeisenDBManager.ConnectToMySQL(
                string.Format(sqlConnectionString,
                AmeisenSettings.Settings.databaseIp,
                AmeisenSettings.Settings.databasePort,
                AmeisenSettings.Settings.databaseName,
                AmeisenSettings.Settings.databaseUsername,
                AmeisenSettings.Settings.databasePasswort)
            );

        /// <summary>
        /// Stops the bots mechanisms, hooks, ...
        /// </summary>
        public void StopBot()
        {
            // Disconnect from Server
            AmeisenClient.Unregister(
                Me,
                IPAddress.Parse(AmeisenSettings.Settings.ameisenServerIp),
                AmeisenSettings.Settings.ameisenServerPort);

            // Save WoW's window positions
            SafeNativeMethods.Rect wowRect = AmeisenCore.GetWowDiemsions(WowExe.process.MainWindowHandle);
            AmeisenSettings.Settings.wowRectT = wowRect.Top;
            AmeisenSettings.Settings.wowRectB = wowRect.Bottom;
            AmeisenSettings.Settings.wowRectL = wowRect.Left;
            AmeisenSettings.Settings.wowRectR = wowRect.Right;

            // Stop object updates
            AmeisenObjectManager.Stop();

            // Stop the statemachine
            AmeisenStateMachineManager.Stop();

            // Unhook Events
            AmeisenEventHook?.Stop();

            // Unhook the EndScene
            AmeisenHook.DisposeHooking();

            // Detach BlackMagic, causing weird crash right now...
            //Blackmagic.Close();

            // Stop logging
            AmeisenLogger.Instance.StopLogging();
            AmeisenSettings.SaveToFile(AmeisenSettings.loadedconfName);
        }

        /// <summary>
        /// Add a RememberedUnit to the RememberedUnits Database to remember its position and UnitTraits
        /// </summary>
        /// <param name="rememberedUnit">Unit that you want to remember</param>
        public void RememberUnit(RememberedUnit rememberedUnit)
            => AmeisenDBManager.RememberUnit(rememberedUnit);

        /// <summary>
        /// Check if we remember a Unit by its Name, ZoneID and MapID
        /// </summary>
        /// <param name="name">name of the npc</param>
        /// <param name="zoneID">zoneid of the npc</param>
        /// <param name="mapID">mapid of the npc</param>
        /// <returns>RememberedUnit with if we remember it, its UnitTraits and position</returns>
        public RememberedUnit CheckForRememberedUnit(string name, int zoneID, int mapID)
            => AmeisenDBManager.CheckForRememberedUnit(name, zoneID, mapID);

        /// <summary>
        /// Compile a CombatClass *.cs file and return its Instance
        /// </summary>
        /// <param name="combatclassPath">*.cs CombatClass file</param>
        /// <returns>Instance of the built Class, if its null somethings gone wrong</returns>
        private IAmeisenCombatPackage CompileAndLoadCombatClass(string combatclassPath)
        {
            if (File.Exists(combatclassPath))
            {
                try
                {
                    return CompileCombatClass(combatclassPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Compilation error", MessageBoxButton.OK, MessageBoxImage.Error);
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Error while compiling CombatClass: {Path.GetFileName(combatclassPath)}", this);
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"{e.Message}", this);
                }
            }

            return null;
        }

        /// <summary>
        /// Compile a combatclass *.cs file at runtime and load it into the bot
        /// </summary>
        /// <param name="combatclassPath">path to the *.cs file</param>
        /// <returns></returns>
        private IAmeisenCombatPackage CompileCombatClass(string combatclassPath)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Compiling CombatClass: {Path.GetFileName(combatclassPath)}", this);

            CompilerParameters parameters = new CompilerParameters();
            // Include dependencies
            string ownPath = AppDomain.CurrentDomain.BaseDirectory;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Combat.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Utilities.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Logger.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Data.dll");
            parameters.GenerateInMemory = true; // generate no file
            parameters.GenerateExecutable = false; // to output a *.dll not a *.exe

            // compile it
            CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, File.ReadAllText(combatclassPath));

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}\nLine:{error.Line}");
                }

                throw new InvalidOperationException(sb.ToString());
            }

            // Create Instance of CombatClass
            IAmeisenCombatPackage result = (IAmeisenCombatPackage)results.CompiledAssembly.CreateInstance("AmeisenBotCombat.CombatClass");

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Successfully compiled CombatClass: {Path.GetFileName(combatclassPath)}", this);
            return result;
        }
    }
}