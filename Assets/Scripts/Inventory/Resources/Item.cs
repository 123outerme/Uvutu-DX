using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Healing,
    Crystal,
    Armor,
    Weapon,
    //TODO: come up with cool item types
    Other
}

[CreateAssetMenu(fileName = "New Item", menuName = "Uvutu/Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
}