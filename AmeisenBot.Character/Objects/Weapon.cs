using System;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBot.Character.Objects
{
    public class Weapon : Item
    {
        DamageStats DamageStats { get; set; }

        public Weapon(int slot) : base(slot)
        {
            DamageStats = DamageStats.Update();
        }
    }
}
