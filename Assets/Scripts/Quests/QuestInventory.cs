using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    NotStarted,
    InProgress,
    ReadyToTurnInStep,
    Completed
}

public class QuestInventory : MonoBehaviour
{
    public List<QuestTracker> quests;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //manage quest collection (?)
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

        int step = entry.GetCurrentStepProgress();
        if (step >= 0)  //GetCurrentStep() returns null if quest is complete
        {
            if (entry.IsCurrentStepCompleted())  //if the current step has completed progress, that means the player hasn't turned it in yet
                return QuestStatus.ReadyToTurnInStep;
            else
                return QuestStatus.InProgress;  //if the current step isn't completed, we're still working on this step
        }
        else
            return QuestStatus.Completed;  //if the current step index is out of range of the list of steps, this quest is done
    }

    public bool SetQuestStepProgress(string questName, QuestType type, string objectiveName, int accomplishedCount)
    {
        QuestTracker q = GetQuestByName(questName);
        QuestStep step = q.GetCurrentStepInfo();
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
            QuestStep step = curQuest.GetCurrentStepInfo();
            if (step != null && step.turnInName == turnInName && curQuest.IsCurrentStepCompleted())  //if this step turns in to the NPC we are checking for, and it is completed
            {
                pairs.Add(new QuestAndStepPair(step, curQuest.quest));  //add to the list of steps that are about to be turned in
            }
        }
        return pairs;
    }

    public void TurnInCurrentQuestStep(string questName)
    {
        QuestTracker q = GetQuestByName(questName);
        QuestStep step = q.GetCurrentStepInfo();
        if (step != null)
        {
            //TODO: reward player for quest step completion
            Debug.Log("Completed a quest step for " + q.name);
            q.GotoNextStep();
        }
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

    public void ProgressTalkQuest(string npcName)
    {
        foreach(QuestTracker curQuest in quests)
        {
            QuestStep step = curQuest.GetCurrentStepInfo();
            if (step != null && step.type == QuestType.Talk && step.objectiveName == npcName && !curQuest.IsCurrentStepCompleted())
            {
                curQuest.AddProgressToCurrentStep(1);  //add 1 instance of talking as progress!
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
}
