using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public string npcName = "Coconut Head";
    public bool enableDialogue = true;
    public bool readyDialogue = false;
    public bool inDialogue = false;

    public float dialogProgressTime = 0;

    //public Dictionary<string, string> dialogueOptions = new Dictionary<string, string>();
    public List<string> dialogue = new List<string>();
    public int dialogueItem = 0;

    public Quest[] quests;
    public bool givesQuests = false;
    private List<Quest> questsToGive = new List<Quest>();  //quests to give the player upon completion of dialogue reading
    private List<QuestAndStepPair> turningInQuestSteps;  //quests the player is turning in upon completion of dialogue reading
    private List<string> curDialogueList = new List<string>();  //the list of all lines of dialogue to present

    private NPCMovement movement;
    private bool prevEnableMoveSetting;
    
    private PlayerController pController;
    private Quests questsHandler;
    private TMP_Text dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        //get some player scripts
        GameObject player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        questsHandler = player.GetComponent<Quests>();
        
        //get movement script for use in pausing movement while dialogue is active
        movement = GetComponent<NPCMovement>();
        prevEnableMoveSetting = movement.enableMovement;
        
        //get dialogue text component for showing dialogue text
        dialogueText = GameObject.Find("Canvas").transform.Find("Dialogue").gameObject.GetComponent<TMP_Text>();
        
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
                    questsHandler.ProgressTalkQuest(npcName);

                    //get all quest steps that are being turned in
                    turningInQuestSteps = questsHandler.GetQuestStepsForTurnIn(npcName);                
                    foreach(QuestAndStepPair pair in turningInQuestSteps)
                        curDialogueList.AddRange(pair.step.turnInDialogue); //add all dialogues for all quests being turned in currently

                    if (turningInQuestSteps.Count < 1)  //if we aren't turning in any quests
                        curDialogueList.AddRange(dialogue);  //add "normal" dialogue (almost-always present dialogue)
                    
                    foreach(Quest q in quests)
                    {
                        if (questsHandler.GetQuestStatus(q.name) == QuestStatus.NotStarted && questsHandler.ArePrereqsCompleted(q.prerequisiteQuestNames))
                        {
                            curDialogueList.AddRange(q.startDialogue);  //add start dialogue from quest that is about to be added  
                            questsToGive.Add(q);  //add quest to list of quests to be given once dialogue is closed
                        }

                        if (questsHandler.GetQuestStatus(q.name) == QuestStatus.InProgress)
                        {
                            QuestStep curStep = questsHandler.GetQuestByName(q.name).GetCurrentStep();
                            //getting from quest handler so we can get the updated quest details
                            if (curStep != null)
                                curDialogueList.AddRange(curStep.inProgressDialogue); //add in-progress dialogue from currently active quest
                        }
                    }

                    inDialogue = true;
                    pController.SetMovementLock(true);  //lock player movement
                    //show dialogue
                    dialogueText.text = curDialogueList[dialogueItem];  //should be first string shown first
                    //dialogueText.gameObject.transform.position = transform.position;

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
                        dialogueText.text = curDialogueList[dialogueItem];
                    }
                    else
                    { //the NPC is out of things to say
                        inDialogue = false;

                        //give quest(s) (if any)
                        foreach(Quest q in questsToGive)
                            questsHandler.AddQuest(q);
                            
                        //turn in quest(s) (if any)
                        foreach(QuestAndStepPair pair in turningInQuestSteps)
                            questsHandler.TurnInCurrentQuestStep(pair.quest);

                        //reset state to disable dialogue mode changes
                        pController.SetMovementLock(false);  //unlock player movement
                        dialogueItem = 0;  //reset current dialogue string index
                        //hide dialogue
                        dialogueText.text = "";  //reset dialogue text to nothing (hide)
                        questsToGive = new List<Quest>();  //clear list of quests to be given
                        turningInQuestSteps = new List<QuestAndStepPair>();  //clear list of quest steps to turn in
                        curDialogueList = new List<string>();  //clear list of dialogue strings
                        movement.enableMovement = prevEnableMoveSetting; //resume NPC movement (if previous setting was enabled)
                    }
                    dialogProgressTime = Time.realtimeSinceStartup;
                }
            }
        }
    }
}
