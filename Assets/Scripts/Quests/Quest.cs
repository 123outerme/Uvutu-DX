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
public class QuestStep
{
    public string description;
    public QuestType type;
    public int count;
    public string objectiveName;   //what NPC to talk to/type of item to collect/monster to defeat, to include names or "groups" such as World 1, creature types, element types, item types, etc.
    public string turnInName;  //what NPC to turn in at this step - empty string means no turn-in required (rewards get automatically added if any)
    public int progress = 0;  //how many items have been completed
    //hide progress from editor (or do I not, for debug use?)
    public List<string> turnInDialogue = new List<string>(); //turn-in target's step completion dialogue
    public List<string> inProgressDialogue = new List<string>();  //originator NPC's in-progress reminder dialogue
    //TODO: reward information

    public bool IsCompleted()
    {
        return (progress >= count);
    }

    public void AddProgress(int amount)
    {
        progress += amount;
    }
}

[System.Serializable]
public class Quest
{
    [System.NonSerialized] public QuestItem questItem;

    public string name;
    public string description;
    public QuestStep[] steps;
    public int currentStep = 0;  //which step player is on; out of range means whole quest is complete
    //hide current step in editor (or do I not, for debug use?)
    public List<string> startDialogue = new List<string>();  //dialogue NPC gives you when you start the quest
    public List<string> prerequisiteQuestNames = new List<string>();  //whole quests that must be completed before you are given the quest
    

    public void GotoNextStep()
    {
        currentStep++;
    }

    public QuestStep GetCurrentStep()
    {
        if (currentStep < 0 || currentStep >= steps.Length)
            return null;  //quest is completed; get no step

        return steps[currentStep];
    }

    public void LoadQuestFromItem(QuestItem qItem)
    {
        questItem = qItem;
        name = questItem.name;
        description = questItem.description;
        steps = questItem.steps;
        startDialogue = new List<string>(questItem.startDialogue);
        prerequisiteQuestNames = new List<string>(questItem.prerequisiteQuestNames);
    }
}