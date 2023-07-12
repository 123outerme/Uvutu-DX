using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats : MonoBehaviour
{
    public Combatant combatantStats;  // cannot be [System.NonSerialized] because of MissingNo. combatant used to detect if battle is new or loading from previous save/scene

    public string combatantName;  //name
    public int level = 1;  //level
    public int exp = 0;  //exp
    public int health = 20;  //current health
    public int maxHealth = 20;  //maximum health
    public int physAttack = 1;  //attack modifier stat
    public int magicAttack = 1;  //magic attack modifier stat
    public int affinity = 1;  // buff/debuff/status strength modifier stat
    public int resistance = 1;  //damage resistance stat
    public int speed = 1;  //move speed stat

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
            health = combatantStats.health;
            maxHealth = combatantStats.maxHealth;
            physAttack = combatantStats.physAttack;
            magicAttack = combatantStats.magicAttack;
            affinity = combatantStats.affinity;
            resistance = combatantStats.resistance;
            speed = combatantStats.speed;

            if (moveset.Length == 0)
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
        physAttackMultiplier *= m.physAttackMultiplier;
        magicAttackMultiplier *= m.magicAttackMultiplier;
        affinityMultiplier *= affinityMultiplier;
        resistanceMultiplier *= resistanceMultiplier;
        speedMultiplier *= speedMultiplier;
    }
}
