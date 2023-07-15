using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crystal", menuName = "Uvutu/Items/Crystal")]
public class Crystal : Item
{
    public override ItemType Type { get { return ItemType.Crystal; } }

    [SerializeField]
    private string[] scenesUnusuableIn = new string[] {"Overworld"};  //overwriteable in-editor! but not in scripting

    public override string[] ScenesUnusuableIn { get { return scenesUnusuableIn; } }

    public Combatant minion;

    //TODO: crystal-related methods (if any)
}
