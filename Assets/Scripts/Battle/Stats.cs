using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatLine
{
    public int maxHealth = 20;  //maximum health
    public int physAttack = 1;  //attack modifier stat
    public int magicAttack = 1;  //magic attack modifier stat
    public int affinity = 1;  // buff/debuff/status strength modifier stat
    public int resistance = 1;  //damage resistance stat
    public int speed = 1;  //move speed stat

    public StatLine(int maxHp, int physAtk, int magicAtk, int affinityStat, int resistanceStat, int speedStat)
    {
        maxHealth = maxHp;
        physAttack = physAtk;
        magicAttack = magicAtk;
        affinity = affinityStat;
        resistance = resistanceStat;
        speed = speedStat;
    }

    public StatLine Copy()
    {
        return new StatLine(maxHealth, physAttack, magicAttack, affinity, resistance, speed);
    }

    public void Set(StatLine s)
    {
        maxHealth = s.maxHealth;
        physAttack = s.physAttack;
        magicAttack = s.magicAttack;
        affinity = s.affinity;
        resistance = s.resistance;
        speed = s.speed;
    }
}

[System.Serializable]
public class Stats : MonoBehaviour
{
    public Combatant combatantStats;  // cannot be [System.NonSerialized] because of MissingNo. combatant used to detect if battle is new or loading from previous save/scene

    public string combatantName;  //name

    public StatLine statLine = null;
    public int level = 1;  //level
    public int exp = 0;  //exp
    public int health = -1;  //current health (-1 means fill in from max health upon Combatant load)

    public float physAttackMultiplier = 1.0f;  //currently applied physical attack multiplier
    public float magicAttackMultiplier = 1.0f;  //currently applied magic attack multiplier
    public float affinityMultiplier = 1.0f;  //currently applied affinity multiplier
    public float resistanceMultiplier = 1.0f;  //currently applied resistance multiplier
    public float speedMultiplier = 1.0f;  //currently applied speed multiplier

    public string[] moveset;

    private SpriteRenderer spr;

    void Start()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        UpdateStats();   
    }

    void Update()
    {

    }

    public void UpdateStats()
    {
        if (combatantStats != null)
        {
            if (spr != null && combatantStats.sprite != null)
                spr.sprite = combatantStats.sprite;

            combatantName = combatantStats.combatantName;

            level = combatantStats.level;
            
            statLine.maxHealth = combatantStats.statLine.maxHealth;
            if (health == -1)
            {
                //Debug.Log(combatantName + health + " / " + maxHealth);
                statLine = combatantStats.statLine.Copy();
                health = statLine.maxHealth;
            }

            if (moveset == null || moveset.Length == 0)
            {
                moveset = new string[combatantStats.moveset.Length];
                for(int i = 0; i < combatantStats.moveset.Length; i++)
                    moveset[i] = combatantStats.moveset[i].moveName;
            }
        }
    }

    public void ResetMultipliers()
    {
        physAttackMultiplier = 1.0f;  //currently applied physical attack multiplier
        magicAttackMultiplier = 1.0f;  //currently applied magic attack multiplier
        affinityMultiplier = 1.0f;  //currently applied affinity multiplier
        resistanceMultiplier = 1.0f;  //currently applied resistance multiplier
        speedMultiplier = 1.0f;  //currently applied speed multiplier
    }

    public void RecieveMultipliers(Move m)
    {
        physAttackMultiplier += (m.physAttackMultiplier - 1.0f);
        magicAttackMultiplier += (m.magicAttackMultiplier - 1.0f);
        affinityMultiplier += (m.affinityMultiplier - 1.0f);
        resistanceMultiplier += (m.resistanceMultiplier - 1.0f);
        speedMultiplier += (m.speedMultiplier - 1.0f);
    }

    public int PlayerGainExp(int gainedExp, PlayerInfo playerInfo)
    {
        exp += gainedExp;
        
        int neededExp = PlayerCalculateNeededExp();
        int levels = 0;
        while(exp >= neededExp)
        {
            exp -= neededExp;  //subtract needed exp from current exp
            levels++;  //level up!
            neededExp = PlayerCalculateNeededExp();  //recalculate needed exp
        }

        PlayerLevelUp(levels, playerInfo);

        return levels;
    }

    public int PlayerCalculateNeededExp()
    {
        //TODO: exp growth curve
        return 100;
    }

    public void PlayerLevelUp(int levels, PlayerInfo playerInfo)
    {
        //TODO: level up based on stat point gain growth, gain max health, then set health to max
        level += levels;
        playerInfo.statPoints += levels;
        playerInfo.statPtPool += levels;
        health = statLine.maxHealth;
    }
}
