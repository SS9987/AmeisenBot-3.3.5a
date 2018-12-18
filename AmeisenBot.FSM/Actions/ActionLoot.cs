using AmeisenBot.Clients;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotDB;
using AmeisenBotUtilities;
using System.Threading;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    public class ActionLoot : ActionMoving
    {
        private AmeisenDataHolder AmeisenDataHolder { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        public ActionLoot(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
        }

        public override void DoThings()
        {
            if(WaypointQueue.Count > 0)
            {
                base.DoThings();
            }

            Unit unitToLoot = AmeisenDataHolder.LootableUnits.Peek();
            double distance = Utils.GetDistance(Me.pos, unitToLoot.pos);

            if(distance > 3)
            {
                UsePathfinding(Me.pos, unitToLoot.pos);
            }
            else
            {
                AmeisenCore.TargetGUID(unitToLoot.Guid);
                AmeisenCore.LuaDoString("InteractUnit(\"target\");");
                Thread.Sleep(1000);
                // We should have looted it by now based on the event
                AmeisenDataHolder.LootableUnits.Dequeue();
            }
        }
    }
}
