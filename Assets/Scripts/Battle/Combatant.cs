using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combatant", menuName = "Uvutu/Combatant")]
public class Combatant : ScriptableObject
{
    public string combatantName;
    public Sprite sprite;
    
    public int level = 1;  //current level
    public StatLine statLine;
    
    public int baseExpYield = 0;  //base exp yielded for defeating as an enemy
    public int baseGoldYield = 0; //base gold yielded for defeating as an enemy

    public Move[] moveset;
}
