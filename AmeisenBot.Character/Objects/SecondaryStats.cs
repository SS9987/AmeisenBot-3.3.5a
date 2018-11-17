namespace AmeisenBot.Character.Objects
{
    public class SecondaryStats
    {
        public double HitRating { get; set; }
        public double EvadeRating { get; set; }
        public double CritRating { get; set; }
        public double BlockRating { get; set; }
        public double Resilience { get; set; }

        public SecondaryStats()
        {
            Update();
        }

        public void Update()
        {
            // TODO: Read the secondary stats
        }
    }
}