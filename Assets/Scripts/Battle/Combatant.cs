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

    public Weapon[] weaponTable;
    public float[] weaponChances;

    public Armor[] armorTable;
    public float[] armorChances;

    public Rewards[] lootTable;
    public float[] lootTableChances;

    public Move[] moveset;
}

public class WeightedRandomChoice
{
    public static int Pick(float[] chances)
    {
        float accumulator = 0.0f;
        float target = Random.Range(0.0f, 1.0f);
        
        for(int i = 0; i < chances.Length; i++)
        {
            accumulator += chances[i];
            if (target <= accumulator)
                return i;
        }

        return -1;  //if -1 is returned, this is probably an error
    }
}
