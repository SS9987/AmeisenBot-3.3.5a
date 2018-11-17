using AmeisenBotCore;
using AmeisenBotLogger;

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

        /// <summary>
        /// Get the stats for yourself
        /// </summary>
        public PrimaryStats()
        {
            UpdateFromPlayer();
        }

        /// <summary>
        /// Get the stats of an item
        /// </summary>
        /// <param name="item">Item to get the stats from</param>
        public PrimaryStats(Item item)
        {
            UpdateFromItem(item);
        }

        public void UpdateFromItem(Item item)
        {
            string itemLink = AmeisenCore.GetLocalizedText(
                $"itemLinkX = GetInventoryItemLink({item.Slot});",
                "itemLinkX"
            );

            string itemStats = AmeisenCore.GetLocalizedText(
                $"itemStatsX = GetItemStats({itemLink});",
                "itemStatsX"
            );

            AmeisenLogger.Instance.Log(LogLevel.WARNING, $"Itemstats of [{item.Id}]: {itemStats}", this);
        }

        public void UpdateFromPlayer()
        {
        }
    }
}