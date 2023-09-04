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

    public Rewards[] lootTable;
    public float[] lootTableChances;

    public Move[] moveset;
}
