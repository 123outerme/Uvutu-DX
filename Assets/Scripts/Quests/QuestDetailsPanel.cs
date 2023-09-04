using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestDetailsPanel : MonoBehaviour
{
    public GameObject stepPanelPrefab;
    public GameObject stepListContent;

    public GameObject player;

    public QuestTracker quest;
    public QuestStep curStep = null;

    public GameObject questTitleObj;
    public GameObject questDescriptionObj;
    public GameObject curStepTitleObj;
    public GameObject curStepDetailObj;
    public GameObject curStepProgressObj;
    public GameObject curStepTurnInObj;
    public GameObject curStepRewardsPanel;

    private TMP_Text questTitleText = null;
    private TMP_Text questDescriptionText = null;
    private TMP_Text curStepTitleText = null;
    private TMP_Text curStepDetailText = null;
    private TMP_Text curStepProgressText = null;
    private TMP_Text curStepTurnInText = null;

    // Start is called before the first frame update
    void Start()
    {
        UpdateReferences();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateReferences()
    {
        if (questTitleText == null)
            questTitleText = questTitleObj.GetComponent<TMP_Text>();

        if (questDescriptionText == null)
            questDescriptionText = questDescriptionObj.GetComponent<TMP_Text>();
        
        if (curStepTitleText == null)
            curStepTitleText = curStepTitleObj.GetComponent<TMP_Text>();
        
        if (curStepDetailText == null)
            curStepDetailText = curStepDetailObj.GetComponent<TMP_Text>();
        
        if (curStepProgressText == null)
            curStepProgressText = curStepProgressObj.GetComponent<TMP_Text>();

        if (curStepTurnInText == null)
            curStepTurnInText = curStepTurnInObj.GetComponent<TMP_Text>();
    }

    public void ViewQuestDetails(QuestTracker q)
    {
        quest = q;
        UpdateReferences();
        curStep = null;

        questTitleText.text = q.quest.name;
        questDescriptionText.text = q.quest.description;

        //destroy each child object in the list to start adding the refreshed list
        foreach(Transform child in stepListContent.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i = q.quest.steps.Length - 1; i >= 0; i--)
        {
            QuestStep step = q.quest.steps[i];

            if (quest.IsStepAchieved(step))
            {
                if (curStep == null)  //view the first (latest) step achieved first
                    ViewQuestStep(step);

                GameObject panelObj = Instantiate(stepPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, stepListContent.transform);
                QuestStepPanel stepPanel = panelObj.GetComponent<QuestStepPanel>();
                
                stepPanel.parentPanel = this;
                stepPanel.step = step;
                stepPanel.shortStepDetail = quest.GetShortStepDetail(step);
                stepPanel.UpdateFromQuestStep();
            }
        }
    }

    public void ViewQuestStep(QuestStep step)
    {
        curStep = step;
        
        curStepTitleText.text = curStep.name;
        curStepDetailText.text = curStep.description;
        curStepProgressText.text = quest.GetStepProgressString(curStep);
        curStepTurnInText.text = curStep.turnInName;

        //update details panel
        TMP_Text goldText = curStepRewardsPanel.transform.Find("GoldDisplay/GoldText").GetComponent<TMP_Text>();
        TMP_Text expText = curStepRewardsPanel.transform.Find("ExpText").GetComponent<TMP_Text>();
        GameObject itemPanel = curStepRewardsPanel.transform.Find("ItemPanel").gameObject;
        Image itemImage = itemPanel.transform.Find("ItemImage").GetComponent<Image>();
        TMP_Text itemNameText = itemPanel.transform.Find("ItemNameText").GetComponent<TMP_Text>();

        expText.text = StatsListPanel.AddCommasToInt(step.rewards.exp) + " Exp";
        goldText.text = StatsListPanel.AddCommasToInt(step.rewards.gold);
        if (step.rewards.item != null)
        {
            itemImage.sprite = step.rewards.item.sprite;
            itemNameText.text = step.rewards.item.ItemName;
            itemPanel.SetActive(true);
        }
        else
        {
            itemPanel.SetActive(false);
        }
    }
}
