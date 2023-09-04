using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsRedeemedStatus
{
    Rewards rewards;
    int levels = 0;
    bool itemAddSuccess = false;

    public RewardsRedeemedStatus(Rewards r, int lvs, bool itemSuccess)
    {
        rewards = r;
        levels = lvs;
        itemAddSuccess = itemSuccess;
    }
}

[System.Serializable]
public class Rewards
{
    public int exp = 0;
    public int gold = 0;

    public Item item = null;
    public string itemName = "";

    public Rewards(int expYield, int goldYield, Item foundItem)
    {
        exp = expYield;
        gold = goldYield;
        item = foundItem;
        LoadRewardItemName();
    }

    public void LoadRewardItemName()
    {
        if (item != null)
            itemName = Item.ItemTypeToString(item.Type) + "/" + item.name;
        else
            itemName = "";
    }

    public void LoadRewardItemFromName()
    {
        if (itemName == "" || itemName == null)
            item = null;
        else
            item = Resources.Load<Item>("Items/" + itemName);
    }
    
    public int RedeemExp(Stats playerStats, PlayerInfo playerInfo)
    {
        return playerStats.PlayerGainExp(exp, playerInfo);
    }

    public void RedeemGold(PlayerInfo playerInfo)
    {
        playerInfo.gold += gold;
    }

    public bool RedeemItem(Inventory playerInventory)
    {
        return playerInventory.AddItemToInventory(item);
    }
}
