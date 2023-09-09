using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Custom Move Effects", menuName = "Uvutu/Move/Custom Move Effects")]
public abstract class MoveEffects : ScriptableObject
{
    public abstract void useMove(Stats[] allies, int userIndex, Stats[] enemies, int[] targetIndices);
}
