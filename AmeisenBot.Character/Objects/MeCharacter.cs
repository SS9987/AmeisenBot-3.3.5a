using AmeisenBotCore;
using AmeisenBotUtilities;

namespace AmeisenBot.Character.Objects
{
    public class MeCharacter
    {
        public PrimaryStats PrimaryStats { get; set; }
        public SecondaryStats SecondaryStats { get; set; }
        public Resistances Resistances { get; set; }

        public int Money { get; set; }

        public Bag[] Bags { get; set; }
        public Equipment Equipment { get; set; }

        public MeCharacter()
        {
            Update();
        }

        public void Update()
        {
            PrimaryStats = new PrimaryStats();
            PrimaryStats.UpdateFromPlayer();
            SecondaryStats = new SecondaryStats();
            Resistances = new Resistances();
            Equipment = new Equipment();
            Bags = new Bag[5] {
                new Bag(0),
                new Bag(1),
                new Bag(2),
                new Bag(3),
                new Bag(4)
            };

            Money = Utils.TryParseInt(AmeisenCore.GetLocalizedText("moneyX = GetMoney();", "moneyX"));
        }
    }
}