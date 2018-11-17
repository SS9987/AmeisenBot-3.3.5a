using System.Collections.Generic;

namespace AmeisenBot.Character.Objects
{
    public class Bag
    {
        public int BagId { get; private set; }
        public string Name { get; set; }
        public int Slots { get; set; }
        public List<Item> Items { get; set; }

        public Bag(int bagId)
        {
            BagId = bagId;
            Update(bagId);
        }

        public void Update(int bagId)
        {
            // TODO: Read the bag
        }
    }
}