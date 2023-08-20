using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public GameObject questSlotPanelPrefab;
    public GameObject questListContent;

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
    }

    public void ClickFilterByStatus(int typeId)
    {
        statusToFilterBy = (QuestStatus) typeId;
        ReloadQuestsDisplay();
    }
}
