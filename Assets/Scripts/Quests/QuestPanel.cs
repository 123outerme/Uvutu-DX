using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public GameObject questSlotPanelPrefab;
    public GameObject questListContent;
    public GameObject questDetailsPanel;

    public QuestStatus statusToFilterBy = QuestStatus.Incomplete;

    public GameObject filterPanel;

    public GameObject player;

    private QuestInventory questInventory;

    // Start is called before the first frame update
    void Start()
    {
        ReloadQuestsDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadQuestsDisplay()
    {
        if (questInventory == null)
            questInventory = player.GetComponent<QuestInventory>();

        //destroy each child object in the list to start adding the refreshed list
        foreach(Transform child in questListContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(QuestTracker q in questInventory.quests)
        {
            QuestStatus qStatus = questInventory.GetQuestStatus(q.name);
            if (statusToFilterBy == QuestStatus.All || (statusToFilterBy == QuestStatus.Incomplete && qStatus != QuestStatus.Completed) || qStatus == statusToFilterBy)
            {
                Debug.Log(qStatus);
                GameObject panelObj = Instantiate(questSlotPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, questListContent.transform);
                QuestSlotPanel questSlotPanel = panelObj.GetComponent<QuestSlotPanel>();
                questSlotPanel.parentPanel = this;
                questSlotPanel.quest = q;
                questSlotPanel.UpdateFromQuestTracker();
            }
        }

        foreach (Transform child in filterPanel.transform)
        {
            Image buttonImage = child.GetComponent<Image>();
            if (QuestInventory.QuestStatusToString(statusToFilterBy).Replace(" ", "") + "FilterButton" == child.name)
                buttonImage.color = new Color(0.678f, 0.847f, 0.902f, 1.0f);  // ADD8E6, 100% alpha - tab's selected color
            else
                buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);  // white, 100% alpha - tab's normal color
        }
    }

    public void ClickFilterByStatus(int typeId)
    {
        statusToFilterBy = (QuestStatus) typeId;
        ReloadQuestsDisplay();
    }

    public void OpenQuestDetails(QuestTracker quest)
    {
        questDetailsPanel.SetActive(true);
        questDetailsPanel.GetComponent<QuestDetailsPanel>().ViewQuestDetails(quest);
    }
}
