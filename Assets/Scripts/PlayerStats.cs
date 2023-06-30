using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health = 20;
    public int maxHealth = 20;
    public int attack = 1;
    public int spAttack = 1;
    public int resistance = 1;
    public int speed = 1;
    public int prestige = 0;
    
    public Vector3 position = new Vector3(-5.5f, 1.5f, 0.0f);
    public string map = "TestMap";

    public string underworldMap = "";

    public bool loaded = false;

    private SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
        saver.Load();
        ApplyPosition();
    }

    public void ApplyPosition()
    {
        transform.position = position;
    }
}
