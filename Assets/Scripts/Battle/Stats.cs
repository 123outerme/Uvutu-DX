using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats : MonoBehaviour
{
    public Combatant combatantStats;

    public int level = 1;  //level
    public int exp = 0;  //exp
    public int health = 20;  //current health
    public int maxHealth = 20;  //maximum health
    public int physAttack = 1;  //attack modifier stat
    public int magicAttack = 1;  //magic attack modifier stat
    public int affinity = 1;  // buff/debuff/status strength modifier stat
    public int resistance = 1;  //damage resistance stat
    public int speed = 1;  //move speed stat

    public Move[] moveset;

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
        
            level = combatantStats.level;
            exp = combatantStats.exp;
            health = combatantStats.health;
            maxHealth = combatantStats.maxHealth;
            physAttack = combatantStats.physAttack;
            magicAttack = combatantStats.magicAttack;
            affinity = combatantStats.affinity;
            resistance = combatantStats.resistance;
            speed = combatantStats.speed;
        }
    }
}
