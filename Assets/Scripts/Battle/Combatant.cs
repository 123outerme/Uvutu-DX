using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combatant", menuName = "Uvutu Combatant")]
public class Combatant : ScriptableObject
{
    public string combatantName;
    public Sprite sprite;
    
    public int level = 1;  //current level
    public int exp = 0;  //amount of exp to next level
    public int health = 20;  //current health
    public int maxHealth = 20;  //maximum health
    public int physAttack = 1;  //attack modifier stat
    public int magicAttack = 1;  //magic attack modifier stat
    public int affinity = 1;  // buff/debuff/status strength modifier stat
    public int resistance = 1;  //damage resistance stat
    public int speed = 1;  //move speed stat

    Move[] moveset = {};
}
