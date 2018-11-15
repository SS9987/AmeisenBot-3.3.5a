using System;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBot.Character.Objects
{
    public class DamageStats
    {
        public double MinDamage { get; set; }
        public double MaxDamage { get; set; }
        public double Dps { get; set; }

        public DamageStats Update()
        {
            DamageStats damageStats = new DamageStats();
            // TODO: Read the damage stats
            return damageStats;
        }
    }
}
