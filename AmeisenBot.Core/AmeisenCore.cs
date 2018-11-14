using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using Magic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AmeisenBotCore
{
    /// <summary>
    /// Abstract class that contains various static method's to interact with WoW's memory and the
    /// EndScene hook.
    /// </summary>
    public abstract class AmeisenCore
    {
        public static BlackMagic BlackMagic { get; set; }
        public static AmeisenHook AmeisenHook { get; set; }

        /// <summary>
        /// AntiAFK
        /// </summary>
        public static void AntiAFK() => BlackMagic.WriteInt(Offsets.tickCount, Environment.TickCount);

        /// <summary>
        /// Switch shapeshift forms, use for example "WoWDruid.ShapeshiftForms.Bear"
        /// </summary>
        /// <param name="index">shapeshift index</param>
        public static void CastShapeshift(int index)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Casting ShapeshiftForm:{index}", "AmeisenCore");
            LuaDoString($"CastShapeshiftForm(\"{index}\");");
        }

        /// <summary>
        /// Cast a spell by its name
        /// </summary>
        /// <param name="spellname">spell to cast</param>
        /// <param name="onMyself">cast spell on myself</param>
        public static void CastSpellByName(string spellname, bool onMyself)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Casting Spell:{spellname}", "AmeisenCore");
            if (onMyself)
            {
                LuaDoString($"CastSpellByName(\"{spellname}\", true);");
            }
            else
            {
                LuaDoString($"CastSpellByName(\"{spellname}\");");
            }
        }

        public static bool IsEnemy(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"isEnemy = UnitIsEnemy({luaunit.ToString()}, {otherluaunit.ToString()});", "isEnemy");


        public static bool CanCooperate(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"canCoop = UnitCanCooperate({luaunit.ToString()}, {otherluaunit.ToString()});", "canCoop");


        public static bool CanAttack(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"canAttack = UnitCanAttack({luaunit.ToString()}, {otherluaunit.ToString()});", "canAttack");


        private static bool ParseLuaIntResult(string command, string outputVariable)
        {
            try
            {
                return int.Parse(GetLocalizedText(command, outputVariable)) > 0;
            }
            catch { return false; }
        }

        public static UnitReaction GetUnitReaction(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
        {
            try
            {
                string cmd = $"reaction = UnitReaction({luaunit.ToString()}, {otherluaunit.ToString()});";
                return (UnitReaction)int.Parse(GetLocalizedText(cmd, "reaction"));
            }
            catch { return UnitReaction.NONE; }
        }

        /// <summary>
        /// Let the bot jump by pressing the spacebar once for 20-40ms
        ///
        /// This runs Async.
        /// </summary>
        public static void CharacterJumpAsync()
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, "Jumping", "AmeisenCore");
            new Thread(CharacterJump).Start();
        }

        /// <summary>
        /// Check if the player's world is in a loadingscreen
        /// </summary>
        /// <returns>true if yes, false if no</returns>
        public static bool CheckLoadingScreen() => false;

        /// <summary>
        /// Check if the player's world is loaded
        /// </summary>
        /// <returns>true if yes, false if no</returns>
        public static bool CheckWorldLoaded() => BlackMagic.ReadInt(Offsets.worldLoaded) == 1;

        /// <summary>
        /// Reads all WoWObject out of WoW's ObjectManager
        /// </summary>
        /// <returns>all WoWObjects as a List</returns>
        public static List<WowObject> GetAllWoWObjects()
        {
            List<WowObject> objects = new List<WowObject>();

            uint currentObjectManager = BlackMagic.ReadUInt(Offsets.currentClientConnection);
            currentObjectManager = BlackMagic.ReadUInt(currentObjectManager + Offsets.currentManagerOffset);

            uint activeObject = BlackMagic.ReadUInt(currentObjectManager + Offsets.firstObjectOffset);
            uint objectType = BlackMagic.ReadUInt(activeObject + Offsets.gameobjectTypeOffset);

            // loop through the objects until an object is bigger than 7 or lower than 1 to get all
            // Objects from manager
            while (objectType <= 7 && objectType > 0)
            {
                WowObject wowObject = ReadWoWObjectFromWoW(activeObject, (WowObjectType)objectType);
                wowObject.MapID = GetMapID();
                wowObject.ZoneID = GetZoneID();
                objects.Add(wowObject);

                activeObject = BlackMagic.ReadUInt(activeObject + Offsets.nextObjectOffset);
                objectType = BlackMagic.ReadUInt(activeObject + Offsets.gameobjectTypeOffset);
            }

            return objects;
        }

        /// <summary>
        /// Check for Buffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Buffs of</param>
        /// <returns>returns unit Buffs as string list</returns>
        public static List<string> GetBuffs(LuaUnit LuaUnit)
        {
            List<string> resultLowered = new List<string>();
            StringBuilder cmdBuffs = new StringBuilder();
            cmdBuffs.Append("local buffs, i = { }, 1;");
            cmdBuffs.Append($"local buff = UnitBuff(\"{LuaUnit.ToString()}\", i);");
            cmdBuffs.Append("while buff do\n");
            cmdBuffs.Append("buffs[#buffs + 1] = buff;");
            cmdBuffs.Append("i = i + 1;");
            cmdBuffs.Append($"buff = UnitBuff(\"{LuaUnit.ToString()}\", i);");
            cmdBuffs.Append("end;");
            cmdBuffs.Append("if #buffs < 1 then\n");
            cmdBuffs.Append("buffs = \"\";");
            cmdBuffs.Append("else\n");
            cmdBuffs.Append("activeUnitBuffs = table.concat(buffs, \", \");");
            cmdBuffs.Append("end;");
            string[] buffs = GetLocalizedText(cmdBuffs.ToString(), "activeUnitBuffs").Split(',');

            foreach (string s in buffs)
            {
                resultLowered.Add(s.Trim().ToLower());
            }

            return resultLowered;
        }

        /// <summary>
        /// Check for Debuffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Debuffs of</param>
        /// <returns>returns unit Debuffs as string list</returns>
        public static List<string> GetDebuffs(LuaUnit LuaUnit)
        {
            List<string> resultLowered = new List<string>();
            StringBuilder cmdDebuffs = new StringBuilder();
            cmdDebuffs.Append("local buffs, i = { }, 1;");
            cmdDebuffs.Append($"local buff = UnitDebuff(\"{LuaUnit.ToString()}\", i);");
            cmdDebuffs.Append("while buff do\n");
            cmdDebuffs.Append("buffs[#buffs + 1] = buff;");
            cmdDebuffs.Append("i = i + 1;");
            cmdDebuffs.Append($"buff = UnitDebuff(\"{LuaUnit.ToString()}\", i);");
            cmdDebuffs.Append("end;");
            cmdDebuffs.Append("if #buffs < 1 then\n");
            cmdDebuffs.Append("buffs = \"\";");
            cmdDebuffs.Append("else\n");
            cmdDebuffs.Append("activeUnitDebuffs = table.concat(buffs, \", \");");
            cmdDebuffs.Append("end;");
            string[] debuffs = GetLocalizedText(cmdDebuffs.ToString(), "activeUnitDebuffs").Split(',');

            foreach (string s in debuffs)
            {
                resultLowered.Add(s.Trim().ToLower());
            }

            return resultLowered;
        }

        /// <summary>
        /// Check for Auras/Buffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Auras of</param>
        /// <returns>returns unit Auras as string list</returns>
        public static List<string> GetAuras(LuaUnit luaUnit)
        {
            List<string> result = new List<string>(GetBuffs(luaUnit));
            result.AddRange(GetDebuffs(luaUnit));
            return result;
        }

        /// <summary>
        /// Returns the current combat state
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to check</param>
        /// <returns>true if unit is in combat, false if not</returns>
        public static bool GetCombatState(LuaUnit LuaUnit)
            => ParseLuaIntResult($"affectingCombat = UnitAffectingCombat(\"{LuaUnit.ToString()}\");", "affectingCombat");



        /// <summary>
        /// Set WoW's window position and dimensions by its handle
        /// </summary>
        /// <param name="mainWindowHandle">WoW's windowHandle</param>
        /// <param name="x">x position on screen</param>
        /// <param name="y">y position on screen</param>
        /// <param name="width">window width</param>
        /// <param name="height">window height</param>
        public static void SetWindowPosition(IntPtr mainWindowHandle, int x, int y, int width, int height)
            => SafeNativeMethods.MoveWindow(mainWindowHandle, x, y, height, width, true);

        /// <summary>
        /// Returns WoW's window size as a native RECT struct by a given windowHandle
        /// </summary>
        /// <param name="mainWindowHandle">WoW's windowHandle</param>
        /// <returns>WoW's window size</returns>
        public static SafeNativeMethods.Rect GetWowDiemsions(IntPtr mainWindowHandle)
        {
            SafeNativeMethods.Rect rect = new SafeNativeMethods.Rect();
            SafeNativeMethods.GetWindowRect(mainWindowHandle, ref rect);
            return rect;
        }

        /// <summary>
        /// Get our active Corpse position
        /// </summary>
        /// <returns>corpse position</returns>
        public static Vector3 GetCorpsePosition() => new Vector3
        (
            BlackMagic.ReadFloat(Offsets.corpseX),
            BlackMagic.ReadFloat(Offsets.corpseY),
            BlackMagic.ReadFloat(Offsets.corpseZ)
        );

        /// <summary>
        /// Get Localized Text for command
        /// </summary>
        /// <param name="command">lua command to run</param>
        /// <param name="variable">variable to read</param>
        /// <returns>localized text for the executed functions return value</returns>
        public static string GetLocalizedText(string command, string variable)
        {
            if (command.Length > 0 && variable.Length > 0)
            {
                // allocate memory for our command
                uint argCCCommand = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1);
                BlackMagic.WriteBytes(argCCCommand, Encoding.UTF8.GetBytes(command));

                string[] asmDoString = new string[]
                {
                    $"MOV EAX, {(argCCCommand) }",
                    "PUSH 0",
                    "PUSH EAX",
                    "PUSH EAX",
                    $"CALL {(Offsets.luaDoString)}",
                    "ADD ESP, 0xC",
                    "RETN",
                };

                // allocate memory for our variable
                uint argCC = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1);
                BlackMagic.WriteBytes(argCC, Encoding.UTF8.GetBytes(variable));

                string[] asmLocalText = new string[]
                {
                    $"CALL {(Offsets.clientObjectManagerGetActivePlayerObject)}",
                    "MOV ECX, EAX",
                    "PUSH -1",
                    $"PUSH {(argCC)}",
                    $"CALL {(Offsets.luaGetLocalizedText)}",
                    "RETN",
                };

                HookJob hookJobLocaltext = new HookJob(asmLocalText, true);
                ReturnHookJob hookJobDoString = new ReturnHookJob(asmDoString, false, hookJobLocaltext);

                // add our hook-job to be executed
                AmeisenHook.AddHookJob(ref hookJobDoString);

                // wait for our hook-job to return
                while (!hookJobDoString.IsFinished) { Thread.Sleep(5); }

                // parse the result bytes to a readable string
                string result = Encoding.UTF8.GetString((byte[])hookJobDoString.ReturnValue);
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, "DoString(" + command + "); => " + variable + " = " + result, "AmeisenCore");

                // free our memory
                BlackMagic.FreeMemory(argCCCommand);
                BlackMagic.FreeMemory(argCC);
                return result;
            }
            return "";
        }

        /// <summary>
        /// Get our current MapID
        /// </summary>
        /// <returns>mapid</returns>
        public static int GetMapID() => BlackMagic.ReadInt(Offsets.mapID);

        /// <summary>
        /// Run through the WoWObjectManager and find the BaseAdress corresponding to the given GUID
        /// </summary>
        /// <param name="guid">guid to search for</param>
        /// <returns>BaseAdress of the WoWObject</returns>
        public static uint GetMemLocByGUID(ulong guid, List<WowObject> woWObjects)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Reading: GUID [{guid}]", "AmeisenCore");

            if (woWObjects != null)
            {
                foreach (WowObject obj in woWObjects)
                {
                    if (obj != null && obj.Guid == guid)
                    {
                        return obj.BaseAddress;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the running WoW's in a WoWExe List containing the
        /// logged in playername and Process object.
        /// </summary>
        /// <returns>A list containing all the runnign WoW processes</returns>
        public static List<WowExe> GetRunningWows()
        {
            List<WowExe> wows = new List<WowExe>();
            List<Process> processList = new List<Process>(Process.GetProcessesByName("Wow"));

            foreach (Process p in processList)
            {
                AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Found WoW Process! PID: {p.Id}", "AmeisenCore");

                BlackMagic blackmagic = new BlackMagic(p.Id);
                wows.Add(new WowExe
                {
                    characterName = blackmagic.ReadASCIIString(Offsets.playerName, 12),
                    process = p
                });
                blackmagic.Close();
            }

            return wows;
        }

        /// <summary>
        /// Check if the spell is on cooldown
        /// </summary>
        /// <param name="spell">spellname</param>
        /// <returns>SpellInfo: spellName, castTime, spellCost</returns>
        public static SpellInfo GetSpellInfo(string spell)
        {
            SpellInfo info = new SpellInfo();
            string cmd = $"_, _, _, cost, _, _, castTime, _ = GetSpellInfo(\"{spell}\");";

            info.name = spell; //try { info.name = GetLocalizedText("name"); } catch { info.castTime = -1; }
            try { info.castTime = int.Parse(GetLocalizedText(cmd, "castTime")); } catch { info.castTime = -1; }
            try { info.cost = int.Parse(GetLocalizedText(cmd, "cost")); } catch { info.cost = -1; }

            return info;
        }

        /// <summary>
        /// Get Units casting Info, example check if we can interrupt it or if Unit is casting
        /// </summary>
        /// <param name="player">LuaUnit to check</param>
        /// <returns>CastingInfo: spellname, castEndTime, canInterrupt</returns>
        public static CastingInfo GetUnitCastingInfo(LuaUnit luaunit)
        {
            CastingInfo info = new CastingInfo();
            string cmd = $"name, _, _, _, _, endTime _, _, canInterrupt = UnitCastingInfo(\"{luaunit.ToString()}\");";

            try { info.name = GetLocalizedText(cmd, "name"); } catch { info.name = "none"; }
            try { info.endTime = int.Parse(GetLocalizedText(cmd, "endTime")); } catch { info.endTime = -1; }
            //try { info.canInterrupt = bool.Parse(GetLocalizedText(cmd, "canInterrupt")); } catch { info.canInterrupt = false; }

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"CastingInfo: [{info.name}, {info.endTime}]", "AmeisenCore");
            return info;
        }

        /// <summary>
        /// Get our active ZoneID
        /// </summary>
        /// <returns>zoneid that wer'e currently in</returns>
        public static int GetZoneID() => BlackMagic.ReadInt(Offsets.zoneID);

        /// <summary>
        /// Move the player to the given guid npc, object or whatever and iteract with it.
        /// </summary>
        /// <param name="pos">Vector3 containing the position to interact with</param>
        /// <param name="guid">guid of the entity</param>
        /// <param name="action">CTM Interaction to perform</param>
        public static void InteractWithGUID(Vector3 pos, ulong guid, InteractionType action)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Interacting[{action}]: X [{pos.X}] Y [{pos.Y}] Z [{pos.Z}] GUID [{guid}]", "AmeisenCore");
            BlackMagic.WriteUInt64(Offsets.ctmGUID, guid);
            MovePlayerToXYZ(pos, action);
        }

        /// <summary>
        /// returns true if the LuaUnit is dead
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit dead</returns>
        public static bool IsDead(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isDead = UnitIsDead(\"{LuaUnit.ToString()}\");", "isDead");


        /// <summary>
        /// returns true if the selected LuaUnit is a ghost or dead
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit a ghost or dead</returns>
        public static bool IsDeadOrGhost(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isDeadOrGhost = UnitIsDeadOrGhost(\"{LuaUnit.ToString()}\");", "isDeadOrGhost");


        /// <summary>
        /// returns true if the LuaUnit is a ghost
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit a ghost</returns>
        public static bool IsGhost(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isGhost = UnitIsDeadOrGhost(\"{LuaUnit.ToString()}\");", "isGhost");


        /// <summary>
        /// Check if the spell is on cooldown
        /// </summary>
        /// <param name="spell">spellname</param>
        /// <returns>true if it is on cooldown, false if not</returns>
        public static bool IsOnCooldown(string spell)
            => ParseLuaIntResult($"start, duration, enabled = GetSpellCooldown(\"{spell}\");", "duration");


        /// <summary>
        /// Returns true or false, wether the Target is friendly or not
        /// </summary>
        /// <returns>true if unit is friendly, false if not</returns>
        public static bool IsFriend(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"isFriendly = UnitIsFriend({luaunit.ToString()}, {otherluaunit.ToString()});", "isFriendly");


        /// <summary>
        /// Execute the given LUA command inside WoW's MainThread
        /// </summary>
        /// <param name="command">lua command to run</param>
        public static void LuaDoString(string command)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Doing string: Command [{command}]", "AmeisenCore");
            // reserve memory for our command and write its bytes to the memory
            uint argCC = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1);
            BlackMagic.WriteBytes(argCC, Encoding.UTF8.GetBytes(command));

            string[] asm = new string[]
            {
                $"MOV EAX, {(argCC)}",
                "PUSH 0",
                "PUSH EAX",
                "PUSH EAX",
                $"CALL {(Offsets.luaDoString)}",
                "ADD ESP, 0xC",
                "RETN",
            };

            // add our hook job to be executed on hook
            HookJob hookJob = new HookJob(asm, false);
            AmeisenHook.AddHookJob(ref hookJob);

            // wait for our hook to return
            while (!hookJob.IsFinished) { Thread.Sleep(1); }

            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Command returned: Command [{command}]", "AmeisenCore");
            BlackMagic.FreeMemory(argCC); // free our codecaves memory
        }

        /// <summary>
        /// Move the Player to the given x, y and z coordinates using MemoryWrite to CTM
        /// </summary>
        /// <param name="pos">Vector3 containing the position to go to</param>
        /// <param name="action">CTM Interaction to perform</param>
        public static void MovePlayerToXYZ(Vector3 pos, InteractionType action, double distance = 1.5)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Moving to: X [{pos.X}] Y [{pos.Y}] Z [{pos.Z}]", "AmeisenCore");
            //if (AmeisenManager.Instance.Me().pos.x != pos.x && AmeisenManager.Instance.Me().pos.y != pos.y && AmeisenManager.Instance.Me().pos.z != pos.z)
            //{
            WriteXYZToMemory(pos, action, (float)distance);
            //}
        }

        /// <summary>
        /// Get the bot's char's GUID
        /// </summary>
        /// <returns>the GUID</returns>
        public static ulong ReadPlayerGUID() => BlackMagic.ReadUInt64(Offsets.localPlayerGUID);

        /// <summary>
        /// Get the bot's char's target's GUID
        /// </summary>
        /// <returns>targets guid</returns>
        public static ulong ReadTargetGUID() => BlackMagic.ReadUInt64(Offsets.localTargetGUID);

        /// <summary>
        /// Read WoWObject from WoW's memory by its BaseAddress
        /// </summary>
        /// <param name="baseAddress">baseAddress of the object</param>
        /// <param name="woWObjectType">objectType of the object</param>
        /// <param name="isMe">reading myself</param>
        /// <returns>the WoWObject</returns>
        public static WowObject ReadWoWObjectFromWoW(uint baseAddress, WowObjectType woWObjectType, bool isMe = false)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Reading: baseAddress [{baseAddress}]", "AmeisenCore");

            if (baseAddress == 0)
            {
                return null;
            }

            switch (woWObjectType)
            {
                case WowObjectType.CONTAINER:
                    return new Container(baseAddress, BlackMagic);

                case WowObjectType.ITEM:
                    return new Item(baseAddress, BlackMagic);

                case WowObjectType.GAMEOBJECT:
                    return new GameObject(baseAddress, BlackMagic);

                case WowObjectType.DYNOBJECT:
                    return new DynObject(baseAddress, BlackMagic);

                case WowObjectType.CORPSE:
                    return new Corpse(baseAddress, BlackMagic);

                case WowObjectType.PLAYER:
                    Player obj = new Player(baseAddress, BlackMagic);

                    if (obj.Guid == ReadPlayerGUID())
                    {
                        // thats me
                        return new Me(baseAddress, BlackMagic);
                    }

                    return obj;

                case WowObjectType.UNIT:
                    return new Unit(baseAddress, BlackMagic);

                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Release charcters spirit to enter ghost state
        /// </summary>
        public static void ReleaseSpirit() => LuaDoString("RepopMe();");

        /// <summary>
        /// Wait until we can recover our corpse and revive our character
        /// </summary>
        public static void RetrieveCorpse(bool checkAndWaitForCorpseDelay)
        {
            if (checkAndWaitForCorpseDelay)
            {
                int corpseDelay = int.Parse(
                GetLocalizedText($"corpseDelay = GetCorpseRecoveryDelay();", "corpseDelay"));
                Thread.Sleep((corpseDelay * 1000) + 100);
            }

            LuaDoString("RetrieveCorpse();");
        }

        /// <summary>
        /// Run the given slash-commando
        /// </summary>
        /// <param name="slashCommand">Example: /target player</param>
        public static void RunSlashCommand(string slashCommand)
            => LuaDoString($"DEFAULT_CHAT_FRAME.editBox:SetText(\"{slashCommand}\") ChatEdit_SendText(DEFAULT_CHAT_FRAME.editBox, 0)");

        /// <summary>
        /// Target a GUID by calling WoW's clientGameUITarget function on our hook
        /// </summary>
        /// <param name="guid">guid to target</param>
        public static void TargetGUID(ulong guid)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"TargetGUID: {guid}", "AmeisenCore");
            //BlackMagic.WriteUInt64(Offsets.localTargetGUID, guid);

            byte[] guidBytes = BitConverter.GetBytes(guid);
            string[] asm = new string[]
            {
                $"PUSH {BitConverter.ToUInt32(guidBytes, 4)}",
                $"PUSH {BitConverter.ToUInt32(guidBytes, 0)}",
                $"CALL {Offsets.clientGameUITarget}",
                "ADD ESP, 0x8",
                "RETN"
            };

            // add our hook-job to process it
            HookJob hookJob = new HookJob(asm, false);
            AmeisenHook.AddHookJob(ref hookJob);

            // wait for the hook-job to return to us
            while (!hookJob.IsFinished) { Thread.Sleep(1); }
        }

        /// <summary>
        /// Let the Character Jump by sending a spacebar key to the game
        /// </summary>
        private static void CharacterJump() => SendKey(new IntPtr(0x20));

        /// <summary>
        /// Hold WoW's main thread, be careful things get dangerous here
        /// </summary>
        private static void PauseMainThread()
        => SThread.SuspendThread(
              SThread.OpenThread(
                  SThread.GetMainThread(BlackMagic.ProcessId).Id));

        /// <summary>
        /// Resumes WoW's main thread
        /// </summary>
        private static void ResumeMainthread()
        => SThread.ResumeThread(
              SThread.OpenThread(
                  SThread.GetMainThread(BlackMagic.ProcessId).Id));

        /// <summary>
        /// Send a vKey to WoW example: "0x20" for Spacebar (VK_SPACE)
        /// </summary>
        /// <param name="vKey">virtual key id</param>
        private static void SendKey(IntPtr vKey)
        {
            const uint KEYDOWN = 0x100;
            const uint KEYUP = 0x101;

            IntPtr windowHandle = BlackMagic.WindowHandle;

            // 0x20 = Spacebar (VK_SPACE)
            SafeNativeMethods.SendMessage(windowHandle, KEYDOWN, vKey, new IntPtr(0));
            Thread.Sleep(new Random().Next(20, 40)); // make it look more human-like :^)
            SafeNativeMethods.SendMessage(windowHandle, KEYUP, vKey, new IntPtr(0));
        }

        /// <summary>
        /// Write the coordinates and action to the memory.
        /// </summary>
        /// <param name="pos">Vector3 containing the position to go to</param>
        /// <param name="action">CTM Interaction to perform</param>
        private static void WriteXYZToMemory(Vector3 pos, InteractionType action, float distance = 1.5f)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Writing: X [{pos.X},{pos.Y},{pos.Z}] Action [{action}] Distance [{distance}]", "AmeisenCore");
            BlackMagic.WriteFloat(Offsets.ctmX, (float)pos.X);
            BlackMagic.WriteFloat(Offsets.ctmY, (float)pos.Y);
            BlackMagic.WriteFloat(Offsets.ctmZ, (float)pos.Z);
            BlackMagic.WriteInt(Offsets.ctmAction, (int)action);
            BlackMagic.WriteFloat(Offsets.ctmDistance, distance);
        }

        /// <summary>
        /// Checks wether you can or can't cast the specific spell right now
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsSpellUseable(string spellname)
            => ParseLuaIntResult($"usable, nomana = IsUsableSpell(\"{spellname}\");", "useable")
            || ParseLuaIntResult($"usable, nomana = IsUsableSpell(\"{spellname}\");", "nomana");
    }
}