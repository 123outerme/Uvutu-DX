using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavernChunk : MonoBehaviour
{
    public int debugId = 0;

    public int depth = 0;
    public int localMaxDepth = 0;

    public string rotationPrefix = "";

    public Dictionary<string, GameObject> exits = new Dictionary<string, GameObject>();

    public string[] exitsKeys = {"", "", "", ""};
    public GameObject[] exitsValues = {null, null, null, null};

    public int timesPrintingVisited = 0;

    public GameObject player;
    public MapLoader maploader;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        maploader = GameObject.Find("MapLoader").GetComponent<MapLoader>();

        //initialize all valid exits to mark that each does not exist
        if (transform.parent.Find("North") == null)
            exits.Add("N", null);
        if (transform.parent.Find("South") == null)
            exits.Add("S", null);
        if (transform.parent.Find("East") == null)
            exits.Add("E", null);
        if (transform.parent.Find("West") == null)
            exits.Add("W", null);
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

        if (maploader != null && maploader.ShouldLoadMoreCavern(depth, localMaxDepth))
        {
            maploader.LoadMoreCavern(transform.parent.gameObject);
        }
    }

    public void PresentDictionary()
    {
        exits.Keys.CopyTo(exitsKeys, 0);
        exits.Values.CopyTo(exitsValues, 0);
    }
}
