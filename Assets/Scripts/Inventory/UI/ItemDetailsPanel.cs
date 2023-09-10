using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailsPanel : MonoBehaviour
{
    public Item item;
    public int itemCount;

    public GameObject itemNameText;
    public GameObject itemTypeText;
    public GameObject itemDescriptionText;
    public GameObject itemImage;
    public GameObject costText;
    public GameObject countText;

    private TMP_Text itemName = null;
    private TMP_Text itemType = null;
    private TMP_Text itemDescription = null;
    private Image itemSprite = null;
    private TMP_Text cost = null;
    private TMP_Text count = null;

    // Start is called before the first frame update
    void Start()
    {
        GetUIScripts();
        if (item != null)
            LoadDetailsFromItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetUIScripts()
    {
        if (itemName == null)
            itemName = itemNameText.GetComponent<TMP_Text>();

        if (itemType == null)
            itemType = itemTypeText.GetComponent<TMP_Text>();

        if (itemDescription == null)
            itemDescription = itemDescriptionText.GetComponent<TMP_Text>();

        if (itemSprite == null)
            itemSprite = itemImage.GetComponent<Image>();

        if (cost == null)
            cost = costText.GetComponent<TMP_Text>();

        if (count == null)
            count = countText.GetComponent<TMP_Text>();
    }

    public void ShowFromItemSlot(InventorySlot slot)
    {
        gameObject.SetActive(true);
        item = slot.item;
        itemCount = slot.count;
        LoadDetailsFromItem();
    }

    public void LoadDetailsFromItem()
    {
        GetUIScripts();

        itemSprite.sprite = item.sprite;
        itemName.text = item.ItemName;
        itemDescription.text = item.Description;
        itemType.text = Item.ItemTypeToString(item.Type);
        
        if (itemCount > 0)
            count.text = "x" + itemCount;
        else
            count.text = "";
        
        cost.text = "" + ((item.Cost > 0) ? item.Cost : "???");
    }
}
