using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Vector3 position = new Vector3(-5.5f, 1.5f, 0.0f);  //start position on new save
    public bool usePosition = true;  //whether this is just present to facilitate saving/loading location in other scenes, or to move the player upon ApplyPosition()
    
    public string map = "TestMap";  //start map on new save
    public string underworldMap = "";  //procedurally-generated map L-System-like string
    public int underworldDepth = 0;  //how deep in the underworld (in number of maps from start state) the player is

    public int prestige = 0;  //how many times the player has New Game+'d (to start)
    public int statPoints = 0;  //how many free stat points the player has (to start)
    public int gold = 10;  //how much money the player has (to start)

    public string scene = "Overworld";  //start scene on new save; this is the current (unpaused) scene the player is in

    public bool inBattle = false;
    public bool exitingBattle = false;

    public bool loaded = false;

    private SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        //saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
        //saver.Load();
        ApplyPosition();
    }

    public void ApplyPosition()
    {
        if (usePosition)
            transform.position = position;
    }

    public void TrySaveUnderworldMap()
    {
        GameObject maploader = GameObject.Find("MapLoader");
        if (maploader != null)
        {
            MapLoader loaderScript = maploader.GetComponent<MapLoader>();
            loaderScript.proceduralGenerator.WriteCavernMapString();
        }
    }
}
