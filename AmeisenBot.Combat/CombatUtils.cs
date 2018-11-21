using AmeisenBotCore;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotCombat
{
    public abstract class CombatUtils
    {
        /// <summary>
        /// Cast a spell
        /// </summary>
        /// <param name="me">you</param>
        /// <param name="target">target to cast the spell on</param>
        /// <param name="spellname">spell to cast</param>
        /// <param name="onMyself">cast sepll on yourself</param>
        /// <param name="waitOnCastToFinish">wait for the cast to finish</param>
        public static void CastSpellByName(Me me, Unit target, string spellname, bool onMyself, bool waitOnCastToFinish = true)
        {
            SpellInfo spellInfo = GetSpellInfo(spellname);

            FaceUnit(me, target);
            AmeisenCore.CastSpellByName(spellname, onMyself);

            if (waitOnCastToFinish)
            {
                Thread.Sleep(100);

                while (AmeisenCore.GetUnitCastingInfo(LuaUnit.player).endTime > 1)
                {
                    Thread.Sleep(25);
                }
            }
        }

        /// <summary>
        /// Get the spellinfo of a spell that contains casttime, mana etc.
        /// </summary>
        /// <param name="spellname">spell to check</param>
        /// <returns>spellinfo</returns>
        public static SpellInfo GetSpellInfo(string spellname)
            => AmeisenCore.GetSpellInfo(spellname);

        public static void TargetNearestEnemy() => AmeisenCore.LuaDoString("TargetNearestEnemy();");

        /// <summary>
        /// Checks for a spell beeing useable
        /// </summary>
        /// <param name="spellname">spell to check for</param>
        /// <returns>wether the spell is useable or not</returns>
        public static bool IsSpellUseable(string spellname)
            => AmeisenCore.IsSpellUseable(spellname);

        /// <summary>
        /// Get a Units buffs & debuffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>buffs & debuffs as a string list</returns>
        public static List<string> GetAuras(LuaUnit luaUnit)
            => AmeisenCore.GetAuras(luaUnit);

        /// <summary>
        /// Get a Units buffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>buffs as a string list</returns>
        public static List<string> GetBuffs(LuaUnit luaUnit)
            => AmeisenCore.GetBuffs(luaUnit);

        /// <summary>
        /// Get a Units debuffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>debuffs as a string list</returns>
        public static List<string> GetDebuffs(LuaUnit luaUnit)
            => AmeisenCore.GetDebuffs(luaUnit);

        /// <summary>
        /// Check for facing a specific unit
        /// </summary
        /// <param name="me">you</param>
        /// <param name="unit">target</param>
        /// <returns>wether you're facing the unit or not</returns>
        public static bool IsFacing(Me me, Unit unit)
            => Utils.IsFacing(me.pos, me.Rotation, unit.pos);

        /// <summary>
        /// Check the LuaUnit for its enemy-state
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit is an enemy or not</returns>
        public static bool IsEnemy(LuaUnit luaUnit)
            => AmeisenCore.IsEnemy(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its friend-state
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit is a friend or not</returns>
        public static bool IsFriend(LuaUnit luaUnit)
            => AmeisenCore.IsFriend(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its attacked to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit can be attacked or not</returns>
        public static bool CanAttack(LuaUnit luaUnit)
            => AmeisenCore.CanAttack(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its coop-state to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit can be cooperated with or not</returns>
        public static bool CanCooperate(LuaUnit luaUnit)
            => AmeisenCore.CanCooperate(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts hostile or not</returns>
        public static bool IsHostile(LuaUnit luaUnit)
            => AmeisenCore.GetUnitReaction(luaUnit) == UnitReaction.HOSTILE;

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts neutral or not</returns>
        public static bool IsNeutral(LuaUnit luaUnit)
            => AmeisenCore.GetUnitReaction(luaUnit) == UnitReaction.NEUTRAL;

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts friendly or not</returns>
        public static bool IsFriendly(LuaUnit luaUnit)
        {
            UnitReaction reaction = AmeisenCore.GetUnitReaction(luaUnit);
            switch (reaction)
            {
                case UnitReaction.FRIENDLY:
                    return true;

                case UnitReaction.HONORED:
                    return true;

                case UnitReaction.REVERED:
                    return true;

                case UnitReaction.EXALTED:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Turn into a specified Units direction
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unit">unit to turn to</param>
        public static void FaceUnit(Me me, Unit unit)
        {
            if (unit != null)
            {
                unit.Update();
                AmeisenCore.MovePlayerToXYZ(
                    unit.pos,
                    InteractionType.FACETARGET,
                    0);
                //AmeisenCore.FaceUnit(me, unit);
            }
        }

        /// <summary>
        /// Check if you're able to cast a certain spell at your current position
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unitToAttack">unit to attack</param>
        /// <param name="distance">spell distance or something alike</param>
        /// <returns>true/false wether your are or arent in range to cast</returns>
        public static bool IsInRange(Me me, Unit unitToAttack, double distance)
            => Utils.GetDistance(me.pos, unitToAttack.pos) < distance;

        /// <summary>
        /// Move into a spell range
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unitToAttack">unit to attack</param>
        /// <param name="distance">distance how close you want to get to the target</param>
        public static void MoveInRange(Me me, Unit unitToAttack, double distance)
        {
            int count = 0;
            while (Utils.GetDistance(me.pos, unitToAttack.pos) > distance && count < 5)
            {
                AmeisenCore.MovePlayerToXYZ(
                        unitToAttack.pos,
                        InteractionType.MOVE,
                        distance);
                me.Update();
                unitToAttack.Update();

                Thread.Sleep(50);
                count++;
            }

            AmeisenCore.InteractWithGUID(
                       unitToAttack.pos,
                       unitToAttack.Guid,
                       InteractionType.STOP);
        }

        public static void AttackTarget() => AmeisenCore.RunSlashCommand("/startattack");

        /// <summary>
        /// Get a target to attack by scanniung hwat your partymembers attack
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="activeWowObjects">all active wow objects</param>
        /// <returns>a possible target unit</returns>
        public static Unit AssistParty(Me me, List<WowObject> activeWowObjects)
        {
            // Get the one with the lowest hp and assist him/her
            List<Unit> units = PartymembersInCombat(me, activeWowObjects);
            if (units.Count > 0)
            {
                Unit u = units.OrderBy(o => o.HealthPercentage).ToList()[0];
                u.Update();

                Unit targetToAttack = (Unit)GetWoWObjectFromGUID(u.TargetGuid, activeWowObjects);
                targetToAttack.Update();

                AmeisenCore.TargetGUID(targetToAttack.Guid);
                me.Update();
                return targetToAttack;
            }
            return null;
        }

        /// <summary>
        /// Return a Player by the given GUID
        /// </summary>
        /// <param name="guid">guid of the player you want to get</param>
        /// <param name="activeWoWObjects">all wowo objects to search in</param>
        /// <returns>Player that you want to get</returns>
        public static WowObject GetWoWObjectFromGUID(ulong guid, List<WowObject> activeWoWObjects)
        {
            foreach (WowObject p in activeWoWObjects)
            {
                if (p.Guid == guid)
                {
                    return p;
                }
            }
            return null;
        }

        public static void MoveToPos(Me me, Unit unitToAttack, double distance = 2.0)
        {
            if (Utils.GetDistance(me.pos, unitToAttack.pos) >= distance)
            {
                AmeisenCore.MovePlayerToXYZ(unitToAttack.pos, InteractionType.MOVE);
            }
        }

        /// <summary>
        /// Target a target that needs healing from your party
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="activeWowObjects">active wow objects</param>
        /// <returns>a teammember that needs healing</returns>
        public static Unit TargetTargetToHeal(Me me, List<WowObject> activeWowObjects)
        {
            // Get the one with the lowest hp and target him/her
            List<Unit> units = PartymembersInCombat(me, activeWowObjects);
            if (units.Count > 0)
            {
                Unit u = units.OrderBy(o => o.HealthPercentage).ToList()[0];
                u.Update();
                AmeisenCore.TargetGUID(u.Guid);
                return u;
            }
            return null;
        }

        /// <summary>
        /// Check if any of our partymembers are in combat
        /// </summary>
        /// <returns>returns all partymembers in combat</returns>
        public static List<Unit> PartymembersInCombat(Me me, List<WowObject> activeWowObjects)
        {
            List<Unit> inCombatUnits = new List<Unit>();
            try
            {
                foreach (ulong guid in me.PartymemberGuids)
                {
                    foreach (WowObject obj in activeWowObjects)
                    {
                        if (guid == obj.Guid && ((Unit)obj).InCombat)
                        {
                            inCombatUnits.Add(((Unit)obj));
                        }
                    }
                }
            }
            catch { }
            return inCombatUnits;
        }
    }
}