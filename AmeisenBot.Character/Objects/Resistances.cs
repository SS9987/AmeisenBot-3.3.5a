namespace AmeisenBot.Character.Objects
{
    public class Resistances
    {
        public double Fire { get; set; }
        public double Frost { get; set; }
        public double Arcane { get; set; }
        public double Nature { get; set; }
        public double Shadow { get; set; }

        public Resistances()
        {
            Update();
        }

        public void Update()
        {
            // TODO: Read the resistances
        }
    }
}