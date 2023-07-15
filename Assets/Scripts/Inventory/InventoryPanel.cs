using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPanel : MonoBehaviour
{
    public GameObject itemSlotPanelPrefab;
    public GameObject itemListContent;

    public ItemType typeToSortBy = ItemType.All;
    public bool lockSort = false;
    public bool disableUseButtonOverride = false;

    public bool inBattle = false;
    public bool inBattleActions = false;

    public GameObject player;

    [System.NonSerialized]
    public PlayerInfo playerInfo = null;
    private Inventory inventory = null;

    public UnityEvent<InventorySlot> useItemCallback;

    // Start is called before the first frame update
    void Start()
    {
        ReloadInventoryDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadInventoryDisplay()
    {
        if (inventory == null)
            inventory = player.GetComponent<Inventory>();

        if (playerInfo == null)
            playerInfo = player.GetComponent<PlayerInfo>();

        InventorySlot[] items = inventory.GetItems();

        //destroy each child object in the list to start adding the refreshed list
        foreach(Transform child in itemListContent.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < items.Length; i++)
        {
            bool battleFiltered = false;
            if (inBattle && !items[i].IsUseAvailable(playerInfo.scene, inBattle, inBattleActions))
                battleFiltered = true;

            if ((typeToSortBy == ItemType.All || items[i].type == typeToSortBy) && !battleFiltered)
            {
                GameObject panelObj = Instantiate(itemSlotPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, itemListContent.transform);
                ItemSlotPanel itemSlotPanel = panelObj.GetComponent<ItemSlotPanel>();
                itemSlotPanel.parentPanel = this;
                itemSlotPanel.itemSlot = items[i];
                itemSlotPanel.UpdateFromItemSlot();
            }
        }
    }

    public void SortByType(ItemType type)
    {
        typeToSortBy = type;
        ReloadInventoryDisplay();
    }
}
