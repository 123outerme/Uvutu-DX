using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleActionType
{
    Move,
    UseItem,
    Escape,
    None
}

[System.Serializable]
public class BattleAction
{
    public BattleActionType type = BattleActionType.None;
    
    [System.NonSerialized]
    public Move move = null;

    public string moveName = "";

    [System.NonSerialized]
    public InventorySlot item = null;

    public string itemName = "";

    public List<string> targetStrings = new List<string>();
    private List<GameObject> targets = new List<GameObject>();
    
    public string nameUser = "";
    private GameObject user = null;

    public BattleAction(GameObject actionUser)
    {
        user = actionUser;
        nameUser = actionUser.name;
    }

    public void FetchMove()
    {
        if (moveName != "")
            move = Resources.Load<Move>("Moves/" + moveName);
    }

    public void LoadItemSlot(InventorySlot slot)
    {
        item = slot;
        itemName = slot.itemName;
        type = BattleActionType.UseItem;
    }

    public void FetchTargetsFromStrings()
    {
        targets = new List<GameObject>();
        foreach(string s in targetStrings)
        {
            targets.Add(GameObject.Find(s));
        }
    }

    public List<GameObject> GetTargetObjs()
    {
        return targets;
    }

    public void FetchUser()
    {
        if (nameUser != "")
            user = GameObject.Find(nameUser);
    }

    public GameObject GetUser()
    {
        if (user == null && nameUser != "")
            FetchUser();

        return user;
    }

    public void FetchAll()
    {
        FetchMove();
        FetchTargetsFromStrings();
        FetchUser();
    }

    public void Clear()
    {
        type = BattleActionType.None;
        move = null;
        moveName = "";
        item = null;
        itemName = "";
        targetStrings = new List<string>();
        targets = new List<GameObject>();
        nameUser = "";
        user = null;
    }
}
