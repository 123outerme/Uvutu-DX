using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnInButton : MonoBehaviour
{
    public GameObject questPanel;
    public string turnInName;

    public NPCDialogue dialogue = null;

    private QuestPanel questPanelScript = null;

    // Start is called before the first frame update
    void Start()
    {
        GetScripts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetScripts()
    {
        if (questPanelScript == null)
            questPanelScript = questPanel.GetComponent<QuestPanel>();
    }

    public void OpenTurnInQuests()
    {
        GetScripts();
        dialogue.showingTurnIn = true;
        questPanelScript.turnInName = turnInName;
        questPanel.SetActive(true);
        questPanelScript.ReloadQuestsDisplay();
    }

    public void CloseTurnInQuests()
    {
        GetScripts();
        dialogue.showingTurnIn = false;
        questPanelScript.turnInName = "";
        questPanel.SetActive(false);
    }
}
