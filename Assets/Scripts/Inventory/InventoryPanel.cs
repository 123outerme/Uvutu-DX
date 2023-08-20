using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GameObject itemSlotPanelPrefab;
    public GameObject itemListContent;

    public ItemType typeToFilterBy = ItemType.All;
    public bool lockFilter = false;
    public bool disableUseButtonOverride = false;

    public bool inBattle = false;
    public bool inBattleActions = false;

    public GameObject player;

    public GameObject filterPanel;

    [System.NonSerialized]
    public PlayerInfo playerInfo = null;
    private Inventory inventory = null;

    public UnityEvent<InventorySlot> useItemCallback;

    public List<ItemType> includedTypes = new List<ItemType>();

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
        includedTypes = new List<ItemType>();

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

            if ((typeToFilterBy == ItemType.All || items[i].type == typeToFilterBy) && !battleFiltered)
            {
                GameObject panelObj = Instantiate(itemSlotPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, itemListContent.transform);
                ItemSlotPanel itemSlotPanel = panelObj.GetComponent<ItemSlotPanel>();
                itemSlotPanel.parentPanel = this;
                itemSlotPanel.itemSlot = items[i];
                itemSlotPanel.UpdateFromItemSlot();
            }


            if (!includedTypes.Contains(items[i].type))
                includedTypes.Add(items[i].type);
        }

        
        foreach (Transform child in filterPanel.transform)
        {
            Button sortButton = child.GetComponent<Button>();

            sortButton.interactable = false;
            foreach(ItemType t in includedTypes)
            {
                if (Item.ItemTypeToString(t).Replace(" ", "") + "FilterButton" == child.name || child.name == "AllFilterButton")
                {
                    sortButton.interactable = true;
                    break;
                }
            }

            if (sortButton.interactable)
                sortButton.interactable = !lockFilter;
        }
    }

    public void FilterByType(ItemType type)
    {
        typeToFilterBy = type;
        ReloadInventoryDisplay();
    }

    public void ClickFilterByType(int typeId)
    {
        ItemType t = (ItemType) typeId;
        FilterByType(t);
    }
}
