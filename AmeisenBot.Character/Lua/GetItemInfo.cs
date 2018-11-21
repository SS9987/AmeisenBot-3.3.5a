/*
--> GetItemInfo LUA Class
> Description:		Get all item information and return it as a JSON
> Parameters:		itemslot = The itemslot number to be scanned (1 = HEAD, ...)
> Output Lua var:	abotItemInfoResult <-- read this using GetLocalizedText or something alike...
> Example output:
{
    "id": "12345",
    "count": "10",
    "quality": "2",
    "curDurability": "40",
    "maxDurability": "100",
    "cooldownStart": "1",
    "cooldownEnd": "5",
    "name": "sampleItem",
    "link": "sampleLink",
    "level": "260",
    "minLevel": "80",
    "type": "sampletype",
    "subtype": "sampleSubtype",
    "maxStack": "80",
    "equiplocation": "sampleEquiplocation",
    "sellprice": "500",
    "classId": "6",
    "subclassId": "7",
    "bindtype": "sampleBindtype",
    "expacId": "666",
    "setId": "855",
    "isCraftingReagent": "0"
}
*/

namespace AmeisenBot.Character.Lua
{
    public static class GetItemInfo
    {
        public static string Lua(int itemslot) => $"abotItemSlot={itemslot}abotItemInfoResult='ERROR'abId=GetInventoryItemID('player',abotItemSlot)abCount=GetInventoryItemCount('player',abotItemSlot)abQuality=GetInventoryItemQuality('player',abotItemSlot)abCurrentDurability,abMaxDurability=GetInventoryItemDurability('player',abotItemSlot)abCooldownStart,abCooldownEnd=GetInventoryItemCooldown('player',abotItemSlot)abName,abLink,abRarity,abLevel,abMinLevel,abType,abSubType,abStackCount,abEquipLoc,abIcon,abSellPrice,abClassID,abSubClassID,abBindType,abExpacID,abSetID,abIsCraftingReagent=GetItemInfo(abId)abotItemInfoResult='{{'..'\"id\": \"'..abId..'\",'..'\"count\": \"'..abCount..'\",'..'\"quality\": \"'..abQuality..'\",'..'\"curDurability\": \"'..abCurrentDurability..'\",'..'\"maxDurability\": \"'..abMaxDurability..'\",'..'\"cooldownStart\": \"'..abCooldownStart..'\",'..'\"cooldownEnd\": '..abCooldownEnd..','..'\"name\": \"'..abName..'\",'..'\"link\": \"'..abLink..'\",'..'\"level\": \"'..abLevel..'\",'..'\"minLevel\": \"'..abMinLevel..'\",'..'\"type\": \"'..abType..'\",'..'\"subtype\": \"'..abSubType..'\",'..'\"maxStack\": \"'..abStackCount..'\",'..'\"equiplocation\": \"'..abEquipLoc..'\",'..'\"sellprice\": \"'..abSellPrice..'\",'..'\"classId\": \"'..abClassId..'\",'..'\"subclassId\": \"'..abSubClassID..'\",'..'\"bindtype\": \"'..abBindType..'\",'..'\"expacId\": \"'..abExpacID..'\",'..'\"setId\": \"'..abSetID..'\",'..'\"isCraftingReagent\": \"'..abIsCraftingReagent..'\"'..'}}'";

        public static string OutVar() => "abotItemInfoResult";
    }
}