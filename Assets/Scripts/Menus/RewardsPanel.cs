using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardsPanel : MonoBehaviour
{
    public Rewards rewards;

    public TMP_Text goldText;
    public TMP_Text expText;
    public GameObject itemPanel;
    public Image itemImage;
    public TMP_Text itemNameText;

    // Start is called before the first frame update
    void Start()
    {
        LoadFromRewardsObj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFromRewardsObj()
    {
        expText.text = StatsListPanel.AddCommasToInt(rewards.exp) + " Exp";
        goldText.text = StatsListPanel.AddCommasToInt(rewards.gold);
        
        if (rewards.item == null)
            rewards.LoadRewardItemFromName();
        
        if (rewards.item != null)
        {
            itemImage.sprite = rewards.item.sprite;
            itemNameText.text = rewards.item.ItemName;
            itemPanel.SetActive(true);
        }
        else
        {
            itemPanel.SetActive(false);
        }
    }
}
