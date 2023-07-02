using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation : MonoBehaviour
{
    public Vector3 position = new Vector3(-5.5f, 1.5f, 0.0f);
    public bool usePosition = true;
    
    public string map = "TestMap";
    public string underworldMap = "";

    public string scene;
    public bool inBattle = false;

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
}
