using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestRewardPanel : MonoBehaviour
{
    public Rewards rewards;

    public GameObject rewardsContainerPanel;
    public GameObject rewardsPanelPrefab;
    public UnityEvent<Item> viewItemDetails;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowRewardsPanel(Rewards reward)
    {
        rewards = reward;
        gameObject.SetActive(true);
        UpdateRewardsPanel();
    }

    public void UpdateRewardsPanel()
    {
        foreach(Transform child in rewardsContainerPanel.transform)
            Destroy(child.gameObject);

        GameObject rewardsPanel = Instantiate(rewardsPanelPrefab, new Vector3(), Quaternion.identity, rewardsContainerPanel.transform) as GameObject;

        RewardsPanel rewardsPanelScript = rewardsPanel.GetComponent<RewardsPanel>();
        rewardsPanelScript.rewards = rewards;
        rewardsPanelScript.viewItemDetails = viewItemDetails;
        rewardsPanelScript.LoadFromRewardsObj();
    }
}
