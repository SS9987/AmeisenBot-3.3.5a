using AmeisenBot.Character.Lua;
using AmeisenBotCore;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AmeisenBot.Character.Objects
{
    public class MeCharacter
    {
        public bool FullyLoaded { get; private set; }
        public PrimaryStats PrimaryStats { get; set; }
        public SecondaryStats SecondaryStats { get; set; }
        public Resistances Resistances { get; set; }

        public int Money { get; set; }

        public List<InventoryItem> InventoryItems { get; set; }
        public Equipment Equipment { get; set; }

        public MeCharacter() { }

        public void Update()
        {
            FullyLoaded = false;
            PrimaryStats = new PrimaryStats();
            PrimaryStats.UpdateFromPlayer();
            SecondaryStats = new SecondaryStats();
            Resistances = new Resistances();
            Equipment = new Equipment();
            InventoryItems = new List<InventoryItem>();

            string inventoryItemsJson = AmeisenCore.GetLocalizedText(GetInventoryItems.Lua(), GetInventoryItems.OutVar());
            List<RawInventoryItem> rawInventoryItems = new List<RawInventoryItem>();

            try
            {
                rawInventoryItems = JsonConvert.DeserializeObject<List<RawInventoryItem>>(inventoryItemsJson);
            }
            catch
            {
                InventoryItems = new List<InventoryItem>();
                AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Failes to parse InventoryItems", this);
            }

            foreach (RawInventoryItem rawInventoryItem in rawInventoryItems)
            {
                InventoryItems.Add(new InventoryItem(rawInventoryItem));
            }

            Money = Utils.TryParseInt(AmeisenCore.GetLocalizedText("moneyX = GetMoney();", "moneyX"));
            FullyLoaded = true;

            string characterJson = JsonConvert.SerializeObject(this);
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Updated Character: {characterJson}", this);
        }
    }
}