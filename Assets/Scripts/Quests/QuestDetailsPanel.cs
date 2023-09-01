using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDetailsPanel : MonoBehaviour
{
    public GameObject stepPanelPrefab;
    public GameObject stepListContent;

    public GameObject player;

    public QuestTracker quest;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ViewQuestDetails(QuestTracker q)
    {
        quest = q;

        foreach(QuestStep step in q.quest.steps)
        {
            //TODO
        }
    }
}
