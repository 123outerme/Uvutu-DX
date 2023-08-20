using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType : int
{
    Healing  = 1,
    Crystal = 2,
    Armor = 3,
    Weapon = 4,
    StoryItem = 5,
    //TODO: come up with cool item types
    Other = 0,
    All = -1  //NOTE: not a type, used for inventory sorting
}

public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string _ItemName;

    [SerializeField]
    private string _Description;

    [SerializeField]
    private int _MaxCarryable = 20;

    [SerializeField]
    private bool _Consumable = true;

    [SerializeField]
    private ValidBattleTarget _ValidTargets = ValidBattleTarget.Allies;

    public string ItemName => _ItemName;
    public string Description => _Description;

    public virtual ItemType Type { get; private set; }

    public int MaxCarryable => _MaxCarryable;

    public bool Consumable => _Consumable;

    public ValidBattleTarget ValidTargets => _ValidTargets;

    public virtual string[] ScenesUnusuableIn { get; set; }

    public Sprite sprite;

    public static string ItemTypeToString(ItemType type)
    {
        if (type == ItemType.All)
            return "All";

        if (type == ItemType.Healing)
            return "Healing";

        if (type == ItemType.Crystal)
            return "Crystal";
        
        if (type == ItemType.Armor)
            return "Armor";

        if (type == ItemType.Weapon)
            return "Weapon";

        if (type == ItemType.StoryItem)
            return "Story Item";

        return "Other";
    }
}