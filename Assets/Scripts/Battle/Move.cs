using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusCondition
{
    None,
    //TODO: create interesting status conditions
}

public enum ValidBattleTarget
{
    Self,  //only valid target is self
    Ally,  //only valid target is 1 other ally
    Allies,  //all units on the allied side are a valid target (single-target)
    AllAllies,  //all allies are targeted (multi-target)
    Enemy,  //only valid target is any 1 enemy
    AllEnemies  //all enemies are targeted (multi-target)
}

public class StatMultiplierText
{
    public string statName;
    public float multiplier;

    public StatMultiplierText(string stat, float statMultiplier)
    {
        statName = stat;
        multiplier = statMultiplier;
    }
}

//[System.Serializable]
[CreateAssetMenu(fileName = "New Move", menuName = "Uvutu/Move")]
public class Move : ScriptableObject
{
    public string moveName;
    public Sprite sprite;

    public float attackPower = 1f;
    public bool isMagic = false;
    public float physAttackMultiplier = 1.0f;
    public float magicAttackMultiplier = 1.0f;
    public float affinityMultiplier = 1.0f;
    public float resistanceMultiplier = 1.0f;
    public float speedMultiplier = 1.0f;
    public StatusCondition appliedCondition = StatusCondition.None;
    public float conditionChance = 0.0f;
    public ValidBattleTarget validTargets = ValidBattleTarget.Enemy;

    public bool HasMultipliers()
    {
        if (physAttackMultiplier != 1.0f)
            return true;

        if (magicAttackMultiplier != 1.0f)
            return true;

        if (affinityMultiplier != 1.0f)
            return true;

        if (resistanceMultiplier != 1.0f)
            return true;

        if (speedMultiplier != 1.0f)
            return true;

        return false;
    }

    public StatMultiplierText[] GetMultipliers()
    {
        List<StatMultiplierText> texts = new List<StatMultiplierText>();

        if (physAttackMultiplier != 1.0f)
            texts.Add(new StatMultiplierText("Phys Attack", physAttackMultiplier));

        if (magicAttackMultiplier != 1.0f)
            texts.Add(new StatMultiplierText("Magic Attack", magicAttackMultiplier));
        
        if (affinityMultiplier != 1.0f)
            texts.Add(new StatMultiplierText("Affinity", affinityMultiplier));
        
        if (resistanceMultiplier != 1.0f)
            texts.Add(new StatMultiplierText("Resistance", resistanceMultiplier));

        if (speedMultiplier != 1.0f)
            texts.Add(new StatMultiplierText("Speed", speedMultiplier));

        return texts.ToArray();
    }
}