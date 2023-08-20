using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestSlotPanel : MonoBehaviour
{
    public QuestPanel parentPanel = null;

    public QuestTracker quest = null;

    private TMP_Text questNameText = null;
    private TMP_Text questStepText = null;

    // Start is called before the first frame update
    void Start()
    {
        UpdateFromQuestTracker();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetComponentReferences()
    {   
        if (questNameText == null)
            questNameText = transform.Find("QuestNameText").GetComponent<TMP_Text>();
        
        if (questStepText == null)
            questStepText = transform.Find("QuestStepText").GetComponent<TMP_Text>();
    }

    public void UpdateFromQuestTracker()
    {
        GetComponentReferences();

        if (quest == null)
            return;
        
        questNameText.text = quest.name;
        questStepText.text = quest.GetShortStepDetail(); 
    }
}
