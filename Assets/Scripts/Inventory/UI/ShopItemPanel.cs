using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemPanel : ItemSlotPanel
{
    private TMP_Text costText = null;
    private Button buyButton = null;

    // Start is called before the first frame update
    void Start()
    {
        
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
        
        if (costText == null)
            costText = transform.Find("CostText").GetComponent<TMP_Text>();

        if (buyButton == null)
            buyButton = transform.Find("BuyButton").GetComponent<Button>();
    }

    new public void UpdateFromItemSlot()
    {
        GetComponentReferences();

        if (itemSlot == null)
            return;

        itemImage.sprite = itemSlot.item.sprite;
        itemNameText.text = itemSlot.itemName;
        itemTypeText.text = Item.ItemTypeToString(itemSlot.type);
        costText.text = "" + itemSlot.cost;

        buyButton.interactable = parentPanel.PlayerHasMoney(itemSlot.cost);
    }

    public void BuyItem()
    {
        parentPanel.BuyShopItem(itemSlot);
    }
}
