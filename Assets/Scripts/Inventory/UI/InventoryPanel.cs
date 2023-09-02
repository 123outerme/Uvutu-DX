using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GameObject itemSlotPanelPrefab;
    public GameObject shopItemPanelPrefab;
    public GameObject itemListContent;

    public ItemType typeToFilterBy = ItemType.All;
    public bool lockFilter = false;
    public bool disableUseButtonOverride = false;

    public bool inBattle = false;
    public bool inBattleActions = false;

    public bool isItemShop = false;
    public bool shopSeePlayerInventory = false;

    public GameObject player;
    public NPCShop npcShop = null;

    public GameObject filterPanel;
    public GameObject switchShopInventoryButton;

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
        GameObject prefab = itemSlotPanelPrefab;

        if (isItemShop && !shopSeePlayerInventory && npcShop != null)
        {
            items = npcShop.GetItems();
            prefab = shopItemPanelPrefab;
        }

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
                GameObject panelObj = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, itemListContent.transform);
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
            Button filterButton = child.GetComponent<Button>();
            Image buttonImage = child.GetComponent<Image>();

            filterButton.interactable = false;
            foreach(ItemType t in includedTypes)
            {
                if (Item.ItemTypeToString(t).Replace(" ", "") + "FilterButton" == child.name || child.name == "AllFilterButton")
                {
                    filterButton.interactable = true;
                    break;
                }
            }

            if (filterButton.interactable)
                filterButton.interactable = !lockFilter;

            if (Item.ItemTypeToString(typeToFilterBy).Replace(" ", "") + "FilterButton" == child.name)
                buttonImage.color = new Color(0.678f, 0.847f, 0.902f, 1.0f);  // ADD8E6, 100% alpha - tab's selected color
            else
                buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);  // white, 100% alpha - tab's normal color
        }

        switchShopInventoryButton.SetActive(isItemShop);
        TMP_Text switchButtonText = switchShopInventoryButton.transform.Find("Text").GetComponent<TMP_Text>();
        switchButtonText.text = (shopSeePlayerInventory) ? "View Shop Items" : "View Your Items";
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

    public void BuyShopItem(InventorySlot item)
    {
        inventory.AddItemToInventory(item.item);
    }

    public bool PlayerHasMoney(int money)
    {
        return (playerInfo.gold >= money);
    }
}
