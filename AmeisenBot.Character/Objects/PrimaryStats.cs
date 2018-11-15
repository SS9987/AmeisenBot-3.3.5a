namespace AmeisenBot.Character.Objects
{
    public class PrimaryStats
    {
        public double Armor { get; set; }
        public double Strenght { get; set; }
        public double Agility { get; set; }
        public double Stamina { get; set; }
        public double Intellect { get; set; }
        public double Spirit { get; set; }

        public PrimaryStats()
        {
            Update();
        }

        public void Update()
        {
            // TODO: Read the primary stats
        }
    }
}
