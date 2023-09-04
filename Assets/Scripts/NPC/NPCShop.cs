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
        GetNPCScripts();
        LoadNPCShop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetNPCScripts()
    {
        inventoryPanel = GameObject.Find("ScreenCanvas").transform.Find("InventoryPanel").GetComponent<InventoryPanel>();
    }

    public void ShowShop(NPCDialogue openerDialogue)
    {
        GetNPCScripts();
        dialogue = openerDialogue;
        inventoryPanel.isItemShop = true;
        inventoryPanel.npcShop = this;
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.ReloadInventoryDisplay();
    }

    public void HideShop()
    {
        //Debug.Log("hide shop");
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
