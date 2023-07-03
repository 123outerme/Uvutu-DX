using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTile : MonoBehaviour
{
    public string map = "";
    public Vector3 position = new Vector3(0, 0, 0);

    private GameObject player;
    private MapLoader maploader;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        maploader = GameObject.Find("MapLoader").GetComponent<MapLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        location.position = position;
        location.map = map;
        Debug.Log("Warp: " + location.map);
        maploader.LoadMap();
        location.ApplyPosition();
    }
}
