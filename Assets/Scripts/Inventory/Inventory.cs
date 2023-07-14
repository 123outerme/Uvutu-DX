using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static int MAX_INVENTORY = 64;

    public List<InventorySlot> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new List<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddItemToInventory(Item i)
    {
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
}
