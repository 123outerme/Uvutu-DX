using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public GameObject itemSlotPanelPrefab;
    public GameObject itemListContent;

    [System.NonSerialized]
    public PlayerInfo playerInfo = null;
    private Inventory inventory = null;
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        inventory = player.GetComponent<Inventory>();
        playerInfo = player.GetComponent<PlayerInfo>();

        ReloadInventoryDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadInventoryDisplay()
    {
        InventorySlot[] items = inventory.GetItems();

        //destroy each child object in the list to start adding the refreshed list
        foreach(Transform child in itemListContent.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < items.Length; i++)
        {
            GameObject panelObj = Instantiate(itemSlotPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, itemListContent.transform);
            ItemSlotPanel itemSlotPanel = panelObj.GetComponent<ItemSlotPanel>();
            itemSlotPanel.parentPanel = this;
            itemSlotPanel.itemSlot = items[i];
            itemSlotPanel.UpdateFromItemSlot();
        }
    }
}
