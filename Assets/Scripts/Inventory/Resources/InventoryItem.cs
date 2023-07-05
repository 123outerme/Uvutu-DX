using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    Item item;  //does not get serialized

    public string itemName;
    public ItemType type;
    public int count;

    public void UseItemOnTarget(GameObject target)
    {
        //TODO
    }
}
