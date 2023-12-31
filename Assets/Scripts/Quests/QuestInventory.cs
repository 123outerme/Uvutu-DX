using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInventory : MonoBehaviour
{
    public List<QuestTracker> quests;

    private GameObject player = null;
    private Stats playerStats;
    private PlayerInfo playerInfo;
    private Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        GetPlayerData();
    }

    // Update is called once per frame
    void Update()
    {
        //manage quest collection (?)
    }

    private void GetPlayerData()
    {
        if (player == null)
            player = GameObject.Find("Player");

        if (player != null)
        {
            if (playerStats == null)
                playerStats = player.GetComponent<Stats>();
            
            if (playerInfo == null)
                playerInfo = player.GetComponent<PlayerInfo>();
            
            if (playerInventory == null)
                playerInventory = player.GetComponent<Inventory>();
        }
    }

    public void AddQuest(Quest q)
    {
        if (GetQuestByName(q.name) == null)
        {
            QuestTracker newQuest = new QuestTracker();
            newQuest.LoadDetailsFromQuest(q);
            quests.Add(newQuest);
        }
    }

    public QuestTracker GetQuestByName(string name)
    {
        foreach(QuestTracker curQuest in quests)
        {
            if (name == curQuest.name)
                return curQuest;
        }
        return null;
    }

    public QuestStatus GetQuestStatus(string name)
    {
        QuestTracker entry = GetQuestByName(name);
        if (entry == null)  //if not found, the player hasn't started it
            return QuestStatus.NotStarted;

        return entry.GetStatus();
    }

    public bool SetQuestStepProgress(string questName, QuestType type, string objectiveName, int accomplishedCount)
    {
        QuestTracker q = GetQuestByName(questName);
        QuestStep step = q.GetCurrentStep();
        if (step.type == type && step.objectiveName == objectiveName)
        {
            q.SetProgressForCurrentStep(accomplishedCount);
            return true;
        }

        return false;
    }

    public List<QuestAndStepPair> GetQuestStepsForTurnIn(string turnInName)
    {
        List<QuestAndStepPair> pairs = new List<QuestAndStepPair>();  //create a list of quest & quest-step pairs (for completing the quest step, telling the quest to go to the next step)
        foreach(QuestTracker curQuest in quests)  //for each quest we have
        {
            QuestStep step = curQuest.GetCurrentStep();
            if (step != null && step.turnInName == turnInName && curQuest.IsCurrentStepCompleted())  //if this step turns in to the NPC we are checking for, and it is completed
            {
                pairs.Add(new QuestAndStepPair(step, curQuest.quest));  //add to the list of steps that are about to be turned in
            }
        }
        return pairs;
    }

    public RewardsRedeemedStatus TurnInCurrentQuestStep(string questName)
    {
        GetPlayerData();
        QuestTracker q = GetQuestByName(questName);
        QuestStep step = q.GetCurrentStep();
        if (step != null)
        {
            //TODO: reward player for quest step completion
            Debug.Log("Completed a quest step for " + q.name);
            int levels = step.rewards.RedeemExp(playerStats, playerInfo);
            step.rewards.RedeemGold(playerInfo);
            bool success = step.rewards.RedeemItem(playerInventory);

            q.GotoNextStep();

            return new RewardsRedeemedStatus(step.rewards, levels, success);
        }

        return null;
    }

    public bool ArePrereqsCompleted(string[] prereqs)
    {
        foreach(string prereqName in prereqs)
        {
            if (GetQuestStatus(prereqName) != QuestStatus.Completed)
                return false;
        }
        return true;
    }

    public void ProgressQuest(string targetName, QuestType type, int count)
    {
        foreach(QuestTracker curQuest in quests)
        {
            QuestStep step = curQuest.GetCurrentStep();
            if (step != null && step.type == type && step.objectiveName == targetName && !curQuest.IsCurrentStepCompleted())
            {
                curQuest.AddProgressToCurrentStep(count);  //add progress!
            }
        }
    }



    public void LoadAllQuestDetails()
    {
        foreach(QuestTracker q in quests)
        {
            q.LoadDetailsByName(q.name);
        }
    }

    public static string QuestStatusToString(QuestStatus q)
    {
        if (q == QuestStatus.All)
            return "All";

        if (q == QuestStatus.Incomplete)
            return "Incomplete";

        if (q == QuestStatus.NotStarted)
            return "Not Started";

        if (q == QuestStatus.InProgress)
            return "In Progress";

        if (q == QuestStatus.ReadyToTurnInStep)
            return "Ready To Turn In";

        if (q == QuestStatus.Completed)
            return "Completed";

        return "";
    }
}
