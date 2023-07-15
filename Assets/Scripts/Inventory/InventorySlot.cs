using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [System.NonSerialized]  //the below field does not get serialized
    public Item item;

    public string itemName;
    public ItemType type;
    public int count = 1;

    public InventorySlot()
    {
        item = null;
        type = ItemType.Other;
        itemName = "Unknown Item";
    }

    public InventorySlot(Item i)
    {
        LoadFromItem(i);
    }

    public void UseItemOnTarget(GameObject target)
    {
        //TODO
    }

    public void LoadFromItem(Item i)
    {
        if (i != null)
        {
            item = i;
            itemName = i.ItemName;
            type = i.Type;
        }
    }

    public void AddItem()
    {
        if (count < item.MaxCarryable)
            count++;
    }

    public void TrashItem()
    {
        if (count > 0)
            count--;
    }

    public bool IsUseAvailable(string scene)
    {
        foreach(string s in item.ScenesUnusuableIn)
        {
            if (s == scene)
                return false;
        }

        return true;
    }
}
