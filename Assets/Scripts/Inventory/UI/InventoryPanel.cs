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
    public GameObject goldText;

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

        //if this is a shop and we are seeing the NPC's inventory, use that as the list of items, and use the shop item prefab instead of the usual
        if (isItemShop && !shopSeePlayerInventory && npcShop != null)
        {
            items = npcShop.GetItems();
            prefab = shopItemPanelPrefab;
        }

        //update gold text
        TMP_Text gText = goldText.GetComponent<TMP_Text>();
        gText.text = "" + StatsListPanel.AddCommasToInt(playerInfo.gold);

        //destroy each child object in the list to start adding the refreshed list
        foreach(Transform child in itemListContent.transform)
        {
            Destroy(child.gameObject);
        }

        includedTypes = new List<ItemType>();
        //get the list of all types included in the inventory
        foreach(InventorySlot item in items)
        {
            if (!includedTypes.Contains(item.type) && !(inBattle && !item.IsUseAvailable(playerInfo.scene, inBattle, inBattleActions)))
                includedTypes.Add(item.type);
        }

        //if the UI is filtering by a type that the inventory does not have anymore, set filter to all
        if (!includedTypes.Contains(typeToFilterBy))
            typeToFilterBy = ItemType.All;

        //create all ItemSlotPanels in the scrolling list
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
                itemSlotPanel.isInShop = isItemShop;
                itemSlotPanel.UpdateFromItemSlot();
            }
        }

        //Set all filter buttons based on if at least one item of the button's type is in the inventory, and set selected button's color
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
        
        //enable/set the text of the switch inventory button present in the shop UI
        switchShopInventoryButton.SetActive(isItemShop);
        TMP_Text switchButtonText = switchShopInventoryButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
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

    public void ClickSwitchInventory()
    {
        shopSeePlayerInventory = !shopSeePlayerInventory;
        ReloadInventoryDisplay();
    }

    public void BuyShopItem(InventorySlot item)
    {
        inventory.AddItemToInventory(item.item);
        if (item.count > 0)
            item.count--;
        playerInfo.gold -= item.cost;
        ReloadInventoryDisplay();
    }

    public void SellToShop(InventorySlot item)
    {
        playerInfo.gold += item.cost;
        npcShop.AddItemToInventory(item.item);
    }

    public bool PlayerHasMoney(int money)
    {
        return (playerInfo.gold >= money);
    }

    public void HideInventoryPanel()
    {
        if (npcShop != null && isItemShop)
            npcShop.HideShop();
    }
}
