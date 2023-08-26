using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavernChunk : MonoBehaviour
{
    public int depth = 0;

    public Dictionary<string, GameObject> exits = new Dictionary<string, GameObject>();

    public GameObject player;
    public MapLoader maploader;

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
        Debug.Log("enter chunk at depth " + depth);
        if (player != null)
        {
            PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
            playerInfo.underworldDepth = depth;
        }

        if (maploader != null && maploader.maxDepth < depth + 2)
        {
            maploader.LoadMoreDepth(depth);
        }
    }
}
