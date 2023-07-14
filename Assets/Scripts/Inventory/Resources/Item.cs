using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Healing,
    Crystal,
    Armor,
    Weapon,
    //TODO: come up with cool item types
    Other
}

public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string _ItemName;

    [SerializeField]
    private string _Description;

    [SerializeField]
    private int _MaxCarryable;

    public string ItemName => _ItemName;
    public string Description => _Description;
    public int MaxCarryable => _MaxCarryable; 

    public virtual ItemType Type { get; private set; }

    public Sprite sprite;
}