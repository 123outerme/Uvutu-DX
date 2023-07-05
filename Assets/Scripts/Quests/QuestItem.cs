using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Uvutu/Quest")]
public class QuestItem : ScriptableObject
{
    new public string name;
    public string description;
    public QuestStep[] steps;
    //hide current step in editor (or do I not, for debug use?)
    public string[] startDialogue;  //dialogue NPC gives you when you start the quest
    public string[] prerequisiteQuestNames;  //whole quests that must be completed before you are given the quest
}