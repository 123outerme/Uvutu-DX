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

    public bool givesQuests = false;
    public Quest[] quests;  //list of all quests this NPC gives
    private List<QuestTracker> questsToGive = new List<QuestTracker>();  //quests to give the player upon completion of dialogue reading
    private List<QuestAndStepPair> turningInQuestSteps;  //quests the player is turning in upon completion of dialogue reading
    private List<string> curDialogueList = new List<string>();  //the list of all lines of dialogue to present

    private NPCMovement movement;
    private bool prevEnableMoveSetting;
    
    private GameObject player;
    private PlayerController pController;
    private QuestInventory questsInventory;
    private TMP_Text dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        //get shop script (if activated)
        if (hasShop)
            shop = gameObject.GetComponent<NPCShop>();

        //get some player scripts
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        questsInventory = player.GetComponent<QuestInventory>();
        
        //get movement script for use in pausing movement while dialogue is active
        movement = GetComponent<NPCMovement>();
        prevEnableMoveSetting = movement.enableMovement;

        //get dialogue text component for showing dialogue text
        dialogueText = GameObject.Find("WorldCanvas").transform.Find("Dialogue").gameObject.GetComponent<TMP_Text>();

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
                    //tell quest handler to add progress to quest steps that involve talking to this NPC (so progress can be checked below on self-referencing talk quests)
                    questsInventory.ProgressQuest(gameObject.name, QuestType.Talk, 1);

                    //get all quest steps that are being turned in
                    turningInQuestSteps = questsInventory.GetQuestStepsForTurnIn(gameObject.name);                
                    foreach(QuestAndStepPair pair in turningInQuestSteps)
                        curDialogueList.AddRange(pair.step.turnInDialogue); //add all dialogues for all quests being turned in currently

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
                    pController.SetMovementLock(true);  //lock player movement
                    UpdateDialogueText();  //show dialogue

                    prevEnableMoveSetting = movement.enableMovement;  //save previous NPC movement setting
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
                        foreach(QuestAndStepPair pair in turningInQuestSteps)
                            questsInventory.TurnInCurrentQuestStep(pair.quest.name);

                        //reset state to disable dialogue mode changes
                        if (!hasShop || shop == null)
                            AfterDialogFinished();
                        else
                            shop.ShowShop(this);
                    }
                    dialogProgressTime = Time.realtimeSinceStartup;
                }
            }
        }
    }

    private void AfterDialogFinished()
    {
        pController.SetMovementLock(false);  //unlock player movement
        dialogueItem = 0;  //reset current dialogue string index
        //hide dialogue
        dialogueText.text = "";  //reset dialogue text to nothing (hide)
        questsToGive = new List<QuestTracker>();  //clear list of quests to be given
        turningInQuestSteps = new List<QuestAndStepPair>();  //clear list of quest steps to turn in
        curDialogueList = new List<string>();  //clear list of dialogue strings
        movement.enableMovement = prevEnableMoveSetting; //resume NPC movement (if previous setting was enabled)
    }

    void UpdateDialogueText()
    {
        dialogueText.text = curDialogueList[dialogueItem];  //should be first string shown first
        Vector3 conversationPosDiff = player.transform.position - transform.position;  //vector from NPC to player

        float newYPos = -1.0f;
        if (conversationPosDiff.y > 0.1)  //player collision box is 0.9, so if we're above the vertical where we bump into a wall above
            newYPos = 1.0f;

        conversationPosDiff = new Vector3(conversationPosDiff.x, newYPos, 0);
        dialogueText.gameObject.transform.position = transform.position - conversationPosDiff;
    }

    public void OnCloseShop()
    {
        AfterDialogFinished();
    }
}
