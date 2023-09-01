using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestAndStepPair
{
    public QuestStep step;
    public Quest quest;

    public QuestAndStepPair(QuestStep s, Quest q)
    {
        step = s;
        quest = q;
    }
}

[System.Serializable]
public class QuestTracker
{
    [System.NonSerialized] public Quest quest;

    public string name;
    public int[] stepProgressCounts = {};
    public int currentStep = 0;  //which step player is on; out of range means whole quest is complete
    //hide current step in editor (or do I not, for debug use?)

    public void GotoNextStep()
    {
        currentStep++;
    }

    public int GetCurrentStepProgress()
    {
        if (currentStep < 0 || currentStep >= stepProgressCounts.Length)
            return -1;  //quest is completed; get no step

        return stepProgressCounts[currentStep];
    }

    public int GetStepProgress(QuestStep step)
    {
        int index = GetQuestStepIndex(step);

        if (index == -1)
            return 0;

        return stepProgressCounts[index];
    }

    public bool IsStepCompleted(QuestStep step)
    {
        if (step == null) 
            return false;

        int index = GetQuestStepIndex(step);

        if (index == -1)
            return false;

        return (stepProgressCounts[index] >= step.count);
    }

    public bool IsCurrentStepCompleted()
    {
        QuestStep curStep = GetCurrentStep();
        if (curStep == null)
            return true;

        return (GetCurrentStepProgress() >= curStep.count);
    }

    public bool IsStepAchieved(QuestStep step)
    {
        if (step == null)  //if a null quest is passed, what is the QuestTracker supposed to do with this?
            return false;

        if (GetCurrentStepProgress() == -1)  //if we have completed the whole quest
            return true;
        
        for(int i = 0; i <= currentStep; i++)
        {
            //check every step up to and including the current one
            if (step.name == quest.steps[i].name && step.description == quest.steps[i].description)
                return true;
        }
        return false;
    }

    public QuestStep GetCurrentStep()
    {
        if (currentStep < 0)
            return null;

        if (currentStep >= stepProgressCounts.Length)
            return quest.steps[currentStep - 1];    

        return quest.steps[currentStep];
    }

    public void AddProgressToCurrentStep(int amount)
    {
        int currentProgress = GetCurrentStepProgress();
        if (currentProgress > -1)
            SetProgressForCurrentStep(currentProgress + amount);
    }

    public void SetProgressForCurrentStep(int amount)
    {
        if (currentStep >= 0 && currentStep < stepProgressCounts.Length)
        {
            int maxCount = GetCurrentStep().count;
            int count = amount;
            if (amount > maxCount)
                count = maxCount;

            stepProgressCounts[currentStep] = count;
        }

        
        //Debug.Log(stepProgressCounts[currentStep]);
    }

    public string GetCurrentShortStepDetail()
    {
        QuestStep step = GetCurrentStep();
        
        return GetShortStepDetail(step);
    }

    public string GetShortStepDetail(QuestStep step)
    {
        if (GetQuestStepIndex(step) < currentStep)  //if quest step is completed already
        {
            step = quest.steps[quest.steps.Length - 1];
            return "Turned in to " + step.turnInName + ".";
        }

        if (IsStepCompleted(step))  //otherwise if step progress is completed, step is ready to be turned in
            return "Turn in to " + step.turnInName + "!";

        return GetStepProgressString(step);
    }

    public string GetStepProgressString(QuestStep step)
    {
        string s = "";

        if (step.type == QuestType.Talk)
            s = "Talk to";

        if (step.type == QuestType.CollectItem)
            s = "Collect";

        if (step.type == QuestType.DefeatMonster)
            s = "Defeat";

        s += " " + step.objectiveName + " (" + GetStepProgress(step)  + " / " + step.count + ")!";
        return s;
    }

    public void LoadDetailsFromQuest(Quest q)
    {
        quest = q;
        name = quest.name;

        if (stepProgressCounts.Length == 0)
        {  //if not loaded from a save, create the integer array for keeping track of progress
            stepProgressCounts = new int[quest.steps.Length];
        }
    }

    public void LoadDetailsByName(string questName)
    {
        LoadDetailsFromQuest(Resources.Load<Quest>("Quests/" + questName));
    }

    private int GetQuestStepIndex(QuestStep step)
    {
        int index = -1;

        for(int i = 0; i < quest.steps.Length; i++)
        {
            if (step.name == quest.steps[i].name && step.description == quest.steps[i].description)
                index = i;
        }

        return index;
    }
}