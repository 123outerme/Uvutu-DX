using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleView
{
    SummonPrompt,
    Command,
    Attack,
    Target,
    UseItem,
    TurnActions,
    FinishBattle,
    LevelUp
}

[System.Serializable]
public class BattleRewards
{
    public int exp;
    public int gold;
    public Item item;

    public BattleRewards(int expYield, int goldYield, Item foundItem)
    {
        exp = expYield;
        gold = goldYield;
        item = foundItem;
    }
}

[System.Serializable]
public class BattleState
{
    public BattleView view;
    public bool commandingMinion;
    public int turnCount;
    public bool battleStarted = false;
    public string selectedTarget;
    public bool battleOverviewAvailable = true;
    public BattleRewards reward = null;

    public BattleState()
    {
        view = BattleView.SummonPrompt;
        commandingMinion = false;
        turnCount = 0;
        battleStarted = false;
        selectedTarget = "";
        battleOverviewAvailable = true;
        reward = null;
    }

    public void IncrementTurn()
    {
        turnCount++;
    }

    public void StartBattle()
    {
        battleStarted = true;
    }

    public void SetSelectedTarget(GameObject target)
    {
        selectedTarget = target.name;
    }
}
