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

    public Rewards(int expYield, int goldYield, Item foundItem)
    {
        exp = expYield;
        gold = goldYield;
        item = foundItem;
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
