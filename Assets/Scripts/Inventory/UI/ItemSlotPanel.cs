using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlotPanel : MonoBehaviour
{
    public InventorySlot itemSlot = null;

    public InventoryPanel parentPanel = null;
    public bool isInShop = false;

    public string playerScene;

    protected Image itemImage = null;
    protected TMP_Text itemNameText = null;
    protected TMP_Text itemTypeText = null;
    protected TMP_Text countText = null;
    protected GameObject costDisplay = null;
    protected TMP_Text costText = null;
    private Button useButton = null;
    private Button trashButton = null;
    private Button sellButton = null;
    private GameObject useActions = null;
    private GameObject shopActions = null;

    void Start()
    {
        UpdateFromItemSlot();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void GetComponentReferences()
    {
        if (itemImage == null)
            itemImage = transform.Find("ItemImage").GetComponent<Image>();
        
        if (itemNameText == null)
            itemNameText = transform.Find("ItemNameText").GetComponent<TMP_Text>();
        
        if (itemTypeText == null)
            itemTypeText = transform.Find("ItemTypeText").GetComponent<TMP_Text>();
        
        if (countText == null)
            countText = transform.Find("CountText").GetComponent<TMP_Text>();
        
        if (costText == null)
            costText = transform.Find("CostDisplay/CostText").GetComponent<TMP_Text>();

        if (useButton == null)
            useButton = transform.Find("UseActions/UseButton").GetComponent<Button>();

        if (trashButton == null)
            trashButton = transform.Find("UseActions/TrashButton").GetComponent<Button>();

        if (sellButton == null)
            sellButton = transform.Find("ShopActions/SellButton").GetComponent<Button>();

        if (costDisplay == null)
            costDisplay = transform.Find("CostDisplay").gameObject;

        if (useActions == null)
            useActions = transform.Find("UseActions").gameObject;

        if (shopActions == null)
            shopActions = transform.Find("ShopActions").gameObject;
    }

    public virtual void UpdateFromItemSlot()
    {
        GetComponentReferences();

        if (itemSlot == null)
            return;

        itemImage.sprite = itemSlot.item.sprite;
        itemNameText.text = itemSlot.itemName;
        itemTypeText.text = Item.ItemTypeToString(itemSlot.type);
        countText.text = "x" + itemSlot.count;
        costText.text = "" + ((itemSlot.cost > 0) ? itemSlot.cost : "???");

        useButton.interactable = (!parentPanel.disableUseButtonOverride && itemSlot.IsUseAvailable(parentPanel.playerInfo.scene, parentPanel.inBattle, parentPanel.inBattleActions));
        trashButton.interactable = itemSlot.item.Consumable;
        if (isInShop)
        {
            sellButton.interactable = (itemSlot.cost > 0);
            useActions.SetActive(false);
            shopActions.SetActive(true);
            costDisplay.SetActive(true);
        }
        else
        {
            useActions.SetActive(true);
            shopActions.SetActive(false);
            costDisplay.SetActive(false);
        }
    }

    public void UseItem()
    {
        //TODO
        parentPanel.useItemCallback.Invoke(itemSlot);
        TrashItem();
    }

    public void TrashItem()
    {
        //TODO instead bring up a confirmation, then trash an item if Yes
        if (itemSlot.item.Consumable)
        {
            itemSlot.TrashItem();
            parentPanel.ReloadInventoryDisplay();
        }
    }

    public void SellItem()
    {
        parentPanel.SellToShop(itemSlot);
        itemSlot.TrashItem();
        parentPanel.ReloadInventoryDisplay();
    }

    public void ViewItemDetails()
    {
        parentPanel.viewItemDetails.Invoke(itemSlot);
    }
}
