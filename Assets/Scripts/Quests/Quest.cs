using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Talk,
    CollectItem,
    DefeatMonster,
}

[System.Serializable]
public class QuestStep
{
    public string name;
    public string description;

    public QuestType type;
    public int count;
    public string objectiveName;   //what NPC to talk to/type of item to collect/monster to defeat, to include names or "groups" such as World 1, creature types, element types, item types, etc.
    public string turnInName;  //what NPC to turn in at this step - empty string means no turn-in required (rewards get automatically added if any)
    public string details;  //the details given by the Quest > Details menu
    
    public List<string> turnInDialogue = new List<string>(); //turn-in target's step completion dialogue
    public List<string> inProgressDialogue = new List<string>();  //originator NPC's in-progress reminder dialogue
    //TODO: reward information
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Uvutu/Quest")]
public class Quest : ScriptableObject
{
    new public string name;
    public string description;
    public QuestStep[] steps;
    //hide current step in editor (or do I not, for debug use?)
    public string[] startDialogue;  //dialogue NPC gives you when you start the quest
    public string[] prerequisiteQuestNames;  //whole quests that must be completed before you are given the quest
}