using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveEffects : MonoBehaviour
{
    public abstract void useMove(Stats[] allies, int userIndex, Stats[] enemies, int[] targetIndices);
}
