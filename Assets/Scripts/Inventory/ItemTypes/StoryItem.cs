using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Story Item", menuName = "Uvutu/Items/Story Item")]
public class StoryItem : Item
{
    public override ItemType Type { get { return ItemType.StoryItem; } }

    [SerializeField]
    private string[] scenesUnusuableIn = new string[] {"Overworld", "Battle"};  //overwriteable in-editor! but not in scripting

    public override string[] ScenesUnusuableIn { get { return scenesUnusuableIn; } }    

    //TODO: general story-item-related methods (if any)
}
