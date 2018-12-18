using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotLogger;
using AmeisenBotMapping.objects;
using AmeisenBotUtilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace AmeisenBotManager
{
    public class AmeisenObjectManager : IDisposable
    {
        private Thread objectUpdateThread;
        private System.Timers.Timer objectUpdateTimer;
        private System.Timers.Timer nodeDBUpdateTimer;
        private DateTime timestampObjects;
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }

        private List<WowObject> ActiveWoWObjects
        {
            get { return AmeisenDataHolder.ActiveWoWObjects; }
            set { AmeisenDataHolder.ActiveWoWObjects = value; }
        }

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

        public AmeisenObjectManager(AmeisenDataHolder ameisenDataHolder, AmeisenDBManager ameisenDBManager)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            RefreshObjects();
        }

        /// <summary>
        /// Get our WoWObjects in the memory
        /// </summary>
        /// <returns>WoWObjects in the memory</returns>
        public List<WowObject> GetObjects()
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, "Getting Objects", this);

            if (ActiveWoWObjects == null)
            {
                RefreshObjectsAsync();
            }

            // need to do this only for specific objects, saving cpu usage
            //if (needToRefresh)
            //RefreshObjectsAsync();
            return ActiveWoWObjects;
        }

        /// <summary>
        /// Return a Player by the given GUID
        /// </summary>
        /// <param name="guid">guid of the player you want to get</param>
        /// <returns>Player that you want to get</returns>
        public WowObject GetWoWObjectFromGUID(ulong guid)
        {
            foreach (WowObject p in ActiveWoWObjects)
            {
                if (p.Guid == guid)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// Starts the ObjectUpdates
        /// </summary>
        public void Start()
        {
            ActiveWoWObjects = AmeisenCore.GetAllWoWObjects();

            foreach (WowObject t in ActiveWoWObjects)
            {
                if (t.GetType() == typeof(Me))
                {
                    Me = (Me)t;
                    break;
                }
            }

            // Timer to update the objects from memory
            objectUpdateTimer = new System.Timers.Timer(AmeisenDataHolder.Settings.dataRefreshRate);
            objectUpdateTimer.Elapsed += ObjectUpdateTimer;
            objectUpdateThread = new Thread(new ThreadStart(StartTimer));
            objectUpdateThread.Start();

            // Timer to update odes in the DB,
            // keep in mind this needs performance at the DB side
            // but will increse mapping details!
            nodeDBUpdateTimer = new System.Timers.Timer(AmeisenDataHolder.Settings.dataRefreshRate);
            nodeDBUpdateTimer.Elapsed += NodeDBUpdateTimer;
            nodeDBUpdateTimer.Start();
        }

        private void NodeDBUpdateTimer(object sender, ElapsedEventArgs e)
        {
            if (Me != null)
            {
                // need lock for this
                UpdateNodeInDB(Me);
            }
        }

        /// <summary>
        /// Stops the ObjectUpdates
        /// </summary>
        public void Stop()
        {
            objectUpdateTimer.Stop();
            objectUpdateThread.Join();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)objectUpdateTimer).Dispose();
                ((IDisposable)nodeDBUpdateTimer).Dispose();
            }
        }

        private void AntiAFK() => AmeisenCore.AntiAFK();

        private void ObjectUpdateTimer(object source, ElapsedEventArgs e) => RefreshObjects();

        private void RefreshObjects()
        {
            ActiveWoWObjects = AmeisenCore.GetAllWoWObjects();

            foreach (WowObject t in ActiveWoWObjects)
            {
                if (t == null && t.Guid == 0)
                {
                    t.Update();
                }

                if (t.GetType() == typeof(Me))
                {
                    t.Update();
                    Me = (Me)t;
                    continue;
                }

                if (Me != null && t.Guid == Me.PetGuid)
                {
                    t.Update();
                    t.Distance = Utils.GetDistance(Me.pos, t.pos);
                    Pet = (Unit)t;
                    continue;
                }

                if (Me != null && t.Guid == Me.TargetGuid)
                {
                    t.Update();
                    if (t.GetType() == typeof(Player))
                    {
                        t.Distance = Utils.GetDistance(Me.pos, t.pos);
                        Target = (Player)t;
                        continue;
                    }
                    else if (t.GetType() == typeof(Unit))
                    {
                        t.Distance = Utils.GetDistance(Me.pos, t.pos);
                        Target = (Unit)t;
                        continue;
                    }
                    else if (t.GetType() == typeof(Me))
                    {
                        Target = (Me)t;
                        continue;
                    }
                }
            }

            // Best place for this :^)
            AntiAFK();
        }

        /// <summary>
        /// Refresh our bot's objectlist, you can get the stats by calling GetObjects().
        ///
        /// This runs Async.
        /// </summary>
        private void RefreshObjectsAsync()
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Refreshing Objects Async", this);
            timestampObjects = DateTime.Now;

            new Thread(new ThreadStart(RefreshObjects)).Start();
        }

        private void StartTimer() => objectUpdateTimer.Start();

        private void UpdateNodeInDB(Me me)
        {
            int zoneID = AmeisenCore.GetZoneID();
            int mapID = AmeisenCore.GetMapID();
            // Me
            AmeisenDBManager.UpdateOrAddNode(new MapNode(me.pos, zoneID, mapID));

            List<ulong> copyOfPartymembers = me.PartymemberGuids;

            // fix with a lock or something alike...
            try
            {
                // All partymembers
                foreach (ulong guid in copyOfPartymembers)
                {
                    Unit unit = (Unit)GetWoWObjectFromGUID(guid);
                    if (unit != null && Utils.GetDistance(me.pos, unit.pos) < 75)
                    {
                        AmeisenDBManager.UpdateOrAddNode(new MapNode(unit.pos, zoneID, mapID));
                    }
                }
            }
            catch { }
        }
    }
}