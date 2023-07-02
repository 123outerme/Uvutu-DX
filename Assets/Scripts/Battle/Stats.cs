using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats : MonoBehaviour
{
    public Combatant combatantStats;

    public int health = 20;  //current health
    public int maxHealth = 20;  //maximum health
    public int attack = 1;  //attack modifier stat
    public int magicAttack = 1;  //magic attack modifier stat
    public int affinity = 1;  // buff/debuff/status strength modifier stat
    public int resistance = 1;  //damage resistance stat
    public int speed = 1;  //move speed stat
    public int prestige = 0;  //number of prestiges

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
        
            health = combatantStats.health;
            maxHealth = combatantStats.maxHealth;
            attack = combatantStats.attack;
            magicAttack = combatantStats.magicAttack;
            affinity = combatantStats.affinity;
            resistance = combatantStats.resistance;
            speed = combatantStats.speed;
            prestige = combatantStats.prestige;
        }
    }
}
