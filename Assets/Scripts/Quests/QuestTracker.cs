using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Talk,
    CollectItem,
    DefeatMonster,
}

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

    public bool IsCurrentStepCompleted()
    {
        QuestStep curStep = GetCurrentStepInfo();
        if (curStep == null)
            return true;

        return (GetCurrentStepProgress() >= curStep.count);
    }

    public QuestStep GetCurrentStepInfo()
    {
        if (currentStep < 0 || currentStep >= stepProgressCounts.Length)
            return null;
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
            stepProgressCounts[currentStep] = amount;

        //Debug.Log(stepProgressCounts[currentStep]);
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
}