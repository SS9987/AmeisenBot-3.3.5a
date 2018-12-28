using AmeisenBotCombat.Interfaces;
using AmeisenBotUtilities;

namespace AmeisenBotCombat.MovementStrategies
{
    public class MovementClose : IMovementStrategy
    {
        private double Distance { get; set; }

        public MovementClose(double distance = 2.0)
        {
            Distance = distance;
        }

        public Vector3 CalculatePosition(Me me, Unit target)
        {
            if (Utils.GetDistance(me.pos, target.pos) > Distance)
            {
                return target.pos;
            }

            return me.pos;
        }
    }
}
