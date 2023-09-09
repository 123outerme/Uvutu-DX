using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffects : MonoBehaviour
{
    public abstract bool useItem(Item i, bool inBattle, bool isBattleAction);  //custom item effects script
}
