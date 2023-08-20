using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemSlotPanel : MonoBehaviour
{
    public InventorySlot itemSlot = null;

    public InventoryPanel parentPanel = null;

    public string playerScene;

    private Image itemImage = null;
    private TMP_Text itemNameText = null;
    private TMP_Text itemTypeText = null;
    private TMP_Text countText = null;
    private Button useButton = null;
    private Button trashButton = null;

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

        if (useButton == null)
            useButton = transform.Find("UseButton").GetComponent<Button>();

        if (trashButton == null)
            trashButton = transform.Find("TrashButton").GetComponent<Button>();
    }

    public void UpdateFromItemSlot()
    {
        GetComponentReferences();

        if (itemSlot == null)
            return;

        itemImage.sprite = itemSlot.item.sprite;
        itemNameText.text = itemSlot.itemName;
        itemTypeText.text = Item.ItemTypeToString(itemSlot.type);
        countText.text = "x" + itemSlot.count;

        useButton.interactable = (!parentPanel.disableUseButtonOverride && itemSlot.IsUseAvailable(parentPanel.playerInfo.scene, parentPanel.inBattle, parentPanel.inBattleActions));
        trashButton.interactable = itemSlot.item.Consumable;
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
}
