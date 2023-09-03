using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static int MAX_INVENTORY = 64;
    public List<InventorySlot> items = new List<InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public InventorySlot[] GetItems()
    {
        RemoveEmptyItemSlots();  //pretty up the inventory first
        return items.ToArray();
    }

    public InventorySlot GetItemSlot(string name)
    {
        InventorySlot slot = null;
        foreach(InventorySlot s in items)
        {
            if (s.itemName == name)
                slot = s;
        }

        return slot;
    }

    public void AddItemToInventory(Item i)
    {
        if (i == null)  //failsafe, do not add a null item
            return;

        foreach(InventorySlot slot in items)
        {
            if (i.ItemName == slot.itemName)
            {
                slot.AddItem();
                return;
            }
        }

        //if the slot where this item belongs isn't found, check the size, if less than max, add an item
        if (items.Count < MAX_INVENTORY)
            items.Add(new InventorySlot(i));    
    }

    public void RemoveEmptyItemSlots()
    {
        List<InventorySlot> itemsToRemove = new List<InventorySlot>();
        foreach(InventorySlot slot in items)
        {
            if (slot.count <= 0)
                itemsToRemove.Add(slot);  //get a list of all items to remove (cannot modify list while iterating over it)
        }

        foreach(InventorySlot rSlot in itemsToRemove)
        {
            items.Remove(rSlot);  //remove the necessary items
        }
    }

    public void LoadAllInventorySlots()
    {
        foreach(InventorySlot slot in items)
        {
            //Debug.Log("Items/" + Item.ItemTypeToString(slot.type) + "/" + slot.itemName);
            slot.LoadFromItem( Resources.Load<Item>("Items/" + Item.ItemTypeToString(slot.type) + "/" + slot.itemName) );
        }
    }
}
