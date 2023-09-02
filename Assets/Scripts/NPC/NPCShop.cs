using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShop : Inventory
{
    public InventoryPanel inventoryPanel;

    // Start is called before the first frame update
    void Start()
    {
        inventoryPanel = GameObject.Find("ScreenCanvas").transform.Find("InventoryPanel").GetComponent<InventoryPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowShop()
    {
        inventoryPanel.isItemShop = true;
        inventoryPanel.npcShop = this;
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.ReloadInventoryDisplay();
    }
}
