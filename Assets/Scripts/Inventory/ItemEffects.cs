using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Custom Item Effects", menuName = "Uvutu/Items/Custom Item Effects")]
public abstract class ItemEffects : ScriptableObject
{
    public abstract bool useItem(Item i, bool inBattle, bool isBattleAction);  //custom item effects script
}
