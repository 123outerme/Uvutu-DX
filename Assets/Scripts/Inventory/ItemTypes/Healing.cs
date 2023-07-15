using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Uvutu/Items/Healing")]
public class Healing : Item
{
    public override ItemType Type { get { return ItemType.Healing; } }

    [SerializeField]
    private string[] scenesUnusuableIn = new string[] {};  //overwriteable in-editor! but not in scripting

    public override string[] ScenesUnusuableIn { get { return scenesUnusuableIn; } }

    public int healthHealedBy = 100;

    //TODO: healing-item-related methods (if any)
}