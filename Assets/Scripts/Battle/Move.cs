using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusCondition
{
    None,
    //TODO: create interesting status conditions
}

[CreateAssetMenu(fileName = "New Move", menuName = "Uvutu Move")]
public class Move : ScriptableObject
{
    string moveName;
    float attackPower = 1f;
    bool isMagic = false;
    float physAttackMultiplier = 1.0f;
    float magicAttackMultiplier = 1.0f;
    float affinityMultiplier = 1.0f;
    float resistanceMultiplier = 1.0f;
    float speedMultiplier = 1.0f;
    StatusCondition appliedCondition = StatusCondition.None;
    float conditionChance = 0.0f;
}
