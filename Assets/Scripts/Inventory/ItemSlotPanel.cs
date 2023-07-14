using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlotPanel : MonoBehaviour
{
    public InventorySlot itemSlot = null;

    public InventoryPanel parentPanel = null;

    private Image itemImage = null;
    private TMP_Text itemNameText = null;
    private TMP_Text itemTypeText = null;
    private TMP_Text countText = null;

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
    }

    public void UpdateFromItemSlot()
    {
        GetComponentReferences();

        itemImage.sprite = itemSlot.item.sprite;
        itemNameText.text = itemSlot.itemName;
        itemTypeText.text = Item.ItemTypeToString(itemSlot.type);
        countText.text = "x" + itemSlot.count;
    }

    public void UseItem()
    {
        //TODO
    }

    public void TrashItem()
    {
        //TODO instead bring up a confirmation, then trash an item if Yes
        itemSlot.TrashItem();
        parentPanel.ReloadInventoryDisplay();
    }
}
