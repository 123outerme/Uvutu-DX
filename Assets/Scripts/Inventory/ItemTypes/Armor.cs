using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Uvutu/Items/Armor")]
public class Armor : Item
{
    public override ItemType Type { get { return ItemType.Armor; } }

    [SerializeField]
    private string[] scenesUnusuableIn = new string[] {};  //overwriteable in-editor! but not in scripting

    public override string[] ScenesUnusuableIn { get { return scenesUnusuableIn; } }

    //TODO: armor-related properties

    //TODO: armor-related methods (if any)
}