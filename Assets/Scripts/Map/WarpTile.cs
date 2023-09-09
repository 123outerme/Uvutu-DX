using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTile : MonoBehaviour
{
    public string map = "";
    public Vector3 position = new Vector3(0, 0, 0);

    public bool loadsUnderground = false;

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
        if (player != null && maploader != null)
        {
            PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
            if (!loadsUnderground)
            {
                playerInfo.position = position;
                playerInfo.map = map;
                //Debug.Log("Warp: " + playerInfo.map);
            }
            else
            {
                playerInfo.savedPosition = playerInfo.position;
                playerInfo.position = new Vector3(0,0,0);
                playerInfo.underworldMap = "new";
            }

            maploader.LoadMap();
            playerInfo.ApplyPosition();
        }
    }
}
