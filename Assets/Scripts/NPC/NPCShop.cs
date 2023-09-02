using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShop : Inventory
{
    public InventoryPanel inventoryPanel;

    private NPCDialogue dialogue = null;

    public Item[] shopItems;
    public int[] counts;

    public bool loaded = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryPanel = GameObject.Find("ScreenCanvas").transform.Find("InventoryPanel").GetComponent<InventoryPanel>();
        LoadNPCShop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowShop(NPCDialogue openerDialogue)
    {
        dialogue = openerDialogue;
        inventoryPanel.isItemShop = true;
        inventoryPanel.npcShop = this;
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.ReloadInventoryDisplay();
    }

    public void HideShop()
    {
        if (dialogue)
            dialogue.OnCloseShop();
        
        inventoryPanel.npcShop = null;
        inventoryPanel.isItemShop = false;
    }

    public void LoadNPCShop()
    {
        if (loaded)
            return;

        for(int i = 0; i < shopItems.Length; i++)
        {
            InventorySlot slot = new InventorySlot(shopItems[i]);
            slot.count = counts[i];
            items.Add(slot);
        }

        loaded = true;
    }
}
