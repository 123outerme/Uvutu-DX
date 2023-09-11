using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public bool enableDialogue = true;
    public bool readyDialogue = false;
    public bool inDialogue = false;

    public float dialogProgressTime = 0;

    //public Dictionary<string, string> dialogueOptions = new Dictionary<string, string>();
    public List<string> dialogue = new List<string>();
    public int dialogueItem = 0;

    public bool hasShop = false;
    private NPCShop shop = null;
    public bool showingTurnInButton = false;
    public bool showingTurnIn = false;
    public bool showingShopButton = false;
    public bool showingShop = false;

    public bool givesQuests = false;
    public Quest[] quests;  //list of all quests this NPC gives
    private List<QuestTracker> questsToGive = new List<QuestTracker>();  //quests to give the player upon completion of dialogue reading
    private List<QuestAndStepPair> turningInQuestSteps;  //quests the player is turning in upon completion of dialogue reading
    private List<string> curDialogueList = new List<string>();  //the list of all lines of dialogue to present

    private NPCMovement movement = null;
    private bool prevEnableMoveSetting;
    
    private GameObject player = null;
    private PlayerController pController = null;
    private QuestInventory questsInventory = null;
    private TMP_Text dialogueText = null;
    private GameObject buttonPanel = null;
    private GameObject shopButton = null;
    private GameObject turnInButton = null;

    // Start is called before the first frame update
    void Start()
    {
        GetAllScripts();

        //set baseline for talk activation delay logic
        dialogProgressTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableDialogue)  //if dialogue features are enabled
        {
            if (pController.readyToSpeak && Time.realtimeSinceStartup - dialogProgressTime > 0.25f) //if player is pressing "Speak" and dialogue actions haven't happened in .25 sec (starting, progressing, or exiting the NPC's dialogue)
            {
                if (!inDialogue && readyDialogue)  //If the NPC is not yet speaking but ready to speak, start dialogue
                {
                    HideButtonPanel();
                    //tell quest handler to add progress to quest steps that involve talking to this NPC (so progress can be checked below on self-referencing talk quests)
                    questsInventory.ProgressQuest(gameObject.name, QuestType.Talk, 1);

                    GetQuestsToTurnInHere();

                    if (turningInQuestSteps.Count < 1)  //if we aren't turning in any quests
                        curDialogueList.AddRange(dialogue);  //add "normal" dialogue (almost-always present dialogue)
                    
                    foreach(Quest q in quests)
                    {
                        if (questsInventory.GetQuestStatus(q.name) == QuestStatus.NotStarted && questsInventory.ArePrereqsCompleted(q.prerequisiteQuestNames))
                        {
                            curDialogueList.AddRange(q.startDialogue);  //add start dialogue from quest that is about to be added  
                            QuestTracker newQuest = new QuestTracker();
                            newQuest.LoadDetailsFromQuest(q);
                            questsToGive.Add(newQuest);  //add quest to list of quests to be given once dialogue is closed
                        }

                        if (questsInventory.GetQuestStatus(q.name) == QuestStatus.InProgress)
                        {
                            QuestStep curStep = questsInventory.GetQuestByName(q.name).GetCurrentStep();
                            //getting from quest handler so we can get the updated quest details
                            if (curStep != null)
                                curDialogueList.AddRange(curStep.inProgressDialogue); //add in-progress dialogue from currently active quest
                        }
                    }

                    inDialogue = true;
                    SetPlayerLock(true);  //lock player movement
                    UpdateDialogueText();  //show dialogue

                    SetPrevEnableMove(movement.enableMovement);  //save previous NPC movement setting
                    movement.enableMovement = false;  //pause NPC movement until dialogue is complete
                    dialogProgressTime = Time.realtimeSinceStartup;  //set last dialogue progression action to now
                }
                else if (inDialogue)  //if the NPC is already speaking, progress dialogue
                {
                    if (dialogueItem + 1 < curDialogueList.Count) //if the NPC has more lines to say
                    {
                        //just show next line
                        dialogueItem++;
                        UpdateDialogueText();  //update dialogue
                    }
                    else
                    { //the NPC is out of things to say
                        inDialogue = false;

                        //give quest(s) (if any)
                        foreach(Quest q in quests)
                            questsInventory.AddQuest(q);
                            
                        //turn in quest(s) (if any)
                        if (turningInQuestSteps.Count > 0)
                            ShowTurnInButton();

                        //reset state to disable dialogue mode changes                        
                        dialogueItem = 0;  //reset current dialogue string index
                        //hide dialogue
                        dialogueText.text = "";  //reset dialogue text to nothing (hide)
                        questsToGive = new List<QuestTracker>();  //clear list of quests to be given
                        turningInQuestSteps = new List<QuestAndStepPair>();  //clear list of quest steps to turn in
                        curDialogueList = new List<string>();  //clear list of dialogue strings

                        if (hasShop && shop != null)
                            ShowShopButton();  //keep the NPC stationary and open the shop button
                        else
                            ReenableMovement();  //otherwise let the NPC move again

                        SetPlayerLock(false);
                            
                    }
                    dialogProgressTime = Time.realtimeSinceStartup;
                }
            }
        }
    }

    public void GetAllScripts()
    {
        //get shop script (if activated)
        if (hasShop && shop == null)
            shop = gameObject.GetComponent<NPCShop>();

        //get some player scripts
        if (player == null)
            player = GameObject.Find("Player");
        
        if (pController == null)
            pController = player.GetComponent<PlayerController>();
        
        if (questsInventory == null)
            questsInventory = player.GetComponent<QuestInventory>();
        
        //get movement script for use in pausing movement while dialogue is active
        if (movement == null)
            movement = GetComponent<NPCMovement>();
        
        //get dialogue text component for showing dialogue text
        if (dialogueText == null)
            dialogueText = GameObject.Find("WorldCanvas/Dialogue").GetComponent<TMP_Text>();
        
        if (buttonPanel == null)
            buttonPanel = GameObject.Find("WorldCanvas/ButtonPanel");

        if (shopButton == null)
            shopButton = buttonPanel.transform.Find("ShopButton").gameObject;

        if (turnInButton == null)
            turnInButton = buttonPanel.transform.Find("TurnInButton").gameObject;
        //have to use Transform.Find to get an inactive object
    }

    public void GetQuestsToTurnInHere()
    {
        //get all quest steps that are being turned in
        GetAllScripts();
        turningInQuestSteps = questsInventory.GetQuestStepsForTurnIn(gameObject.name);
        foreach(QuestAndStepPair pair in turningInQuestSteps)
            curDialogueList.AddRange(pair.step.turnInDialogue); //add all dialogues for all quests being turned in currently
    }

    public void SetPlayerLock(bool playerLock)
    {
        pController.SetMovementLock(playerLock);  //(un)lock player movement
    }

    public void ReenableMovement()
    {
        movement.enableMovement = prevEnableMoveSetting; //resume NPC movement (if previous setting was enabled)
    }

    public void SetCurDialogue(List<string> curDialogue)
    {
        curDialogueList = curDialogue;
    }

    public List<string> GetCurDialogue()
    {
        return curDialogueList;
    }

    public void SetPrevEnableMove(bool prevEnableMove)
    {
        prevEnableMoveSetting = prevEnableMove;
    }

    public bool GetPrevEnableMove()
    {
        return prevEnableMoveSetting;
    }

    public void UpdateDialogueText()
    {
        if (curDialogueList == null || curDialogueList.Count == 0)
            return;

        GetAllScripts();

        dialogueText.text = curDialogueList[dialogueItem];  //should be first string shown first
        Vector3 conversationPosDiff = player.transform.position - transform.position;  //vector from NPC to player

        float newYPos = -1.0f;
        if (conversationPosDiff.y > 0.1)  //player collision box is 0.9, so if we're above the vertical where we bump into a wall above
            newYPos = 1.0f;

        conversationPosDiff = new Vector3(conversationPosDiff.x, newYPos, 0);
        dialogueText.gameObject.transform.position = transform.position - conversationPosDiff;
    }

    public void UpdateButtonPanelPosition()
    {
        GetAllScripts();
        Vector3 conversationPosDiff = player.transform.position - transform.position;  //vector from NPC to player

        float newYPos = -1.0f;
        if (conversationPosDiff.y > 0.1)  //player collision box is 0.9, so if we're above the vertical where we bump into a wall above
            newYPos = 1.0f;

        conversationPosDiff = new Vector3(conversationPosDiff.x, newYPos, 0);
        buttonPanel.transform.position = transform.position - conversationPosDiff;
    }

    public void ShowShopButton()
    {
        GetAllScripts();
        NPCShopButton shopBtnScript = shopButton.GetComponent<NPCShopButton>();
        shopBtnScript.dialogue = this;
        shopBtnScript.shop = shop;
        UpdateButtonPanelPosition();
        shopButton.SetActive(true);
        showingShopButton = true;
        //Debug.Log("show shop button");
    }

    public void HideButtonPanel()
    {
        if (showingShopButton || showingTurnInButton)
        {
            GetAllScripts();
            
            NPCShopButton shopBtnScript = shopButton.GetComponent<NPCShopButton>();
            shopButton.SetActive(false);
            shopBtnScript.dialogue = null;
            shopBtnScript.shop = null;
            showingShopButton = false;
            
            TurnInButton turnInBtnScript = turnInButton.GetComponent<TurnInButton>();
            turnInButton.SetActive(false);
            turnInBtnScript.dialogue = null;
            turnInBtnScript.turnInName = "";
            showingTurnInButton = false;

            ReenableMovement();
        }
    }

    public void OnCloseShop()
    {
        //Debug.Log("on close shop");
        showingShop = false;
        SetPlayerLock(false);
    }

    public void ShowTurnInButton()
    {
        GetAllScripts();
        UpdateButtonPanelPosition();
        turnInButton.SetActive(true);
        showingTurnInButton = true;
        TurnInButton turnInBtnScript = turnInButton.GetComponent<TurnInButton>();
        turnInBtnScript.turnInName = gameObject.name;
        turnInBtnScript.dialogue = this;
    }
}
