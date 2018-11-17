namespace AmeisenBot.Character.Objects
{
    public class Weapon : Item
    {
        private DamageStats DamageStats { get; set; }

        public Weapon(int slot) : base(slot)
        {
            DamageStats = DamageStats.Update();
        }
    }
}