using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType : int
{
    Healing  = 1,
    Crystal = 2,
    Armor = 3,
    Weapon = 4,
    //TODO: come up with cool item types
    Other = 0
}

public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string _ItemName;

    [SerializeField]
    private string _Description;

    [SerializeField]
    private int _MaxCarryable;

    [SerializeField]
    private string[] _ScenesUnusableIn = new string[0];

    public string ItemName => _ItemName;
    public string Description => _Description;
    public int MaxCarryable => _MaxCarryable; 
    public string[] ScenesUnusuableIn => _ScenesUnusableIn;

    public virtual ItemType Type { get; private set; }

    public Sprite sprite;

    public static string ItemTypeToString(ItemType type)
    {
        if (type == ItemType.Healing)
            return "Healing";

        if (type == ItemType.Crystal)
            return "Crystal";
        
        if (type == ItemType.Armor)
            return "Armor";

        if (type == ItemType.Weapon)
            return "Weapon";

        return "Other";
    }
}