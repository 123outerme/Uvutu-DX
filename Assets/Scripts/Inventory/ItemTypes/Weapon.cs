using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Uvutu/Items/Weapon")]
public class Weapon : Item
{
    public override ItemType Type { get { return ItemType.Weapon; } }

    [SerializeField]
    private string[] scenesUnusuableIn = new string[] {};  //overwriteable in-editor! but not in scripting

    public override string[] ScenesUnusuableIn { get { return scenesUnusuableIn; } }

    //TODO: weapon-related properties

    //TODO: weapon-related methods (if any)
}