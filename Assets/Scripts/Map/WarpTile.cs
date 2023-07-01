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
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.position = position;
        stats.map = map;
        Debug.Log("Warp: " + map + " / " + stats.map);
        maploader.LoadMap();
        stats.ApplyPosition();
    }
}
