using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject PlayerInfoParent;
    public bool disable = false;

    private PlayerInfo playerInfo;

    private SaveHandler saver;

    private Dictionary<string, string> exitDict = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = PlayerInfoParent.GetComponent<PlayerInfo>();
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();

        //abbreviations to full names
        exitDict.Add("N", "North");
        exitDict.Add("S", "South");
        exitDict.Add("E", "East");
        exitDict.Add("W", "West");
        //inversion of direction (abbrev -> abbrev)
        exitDict.Add("!N", "S");
        exitDict.Add("!S", "N");
        exitDict.Add("!E", "W");
        exitDict.Add("!W", "E");
        //full inversion of direction (abbrev -> full name)
        exitDict.Add("!~N", "South");
        exitDict.Add("!~S", "North");
        exitDict.Add("!~E", "West");
        exitDict.Add("!~W", "East");

        LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMap()
    {
        if (!disable)
        {
            //Debug.Log("save before loading new map");
            saver.Save();
            
            //destroy each child object in the grid to clear space for the new map
            foreach(Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }

            //Debug.Log(playerInfo.map);

            if (playerInfo.underworldMap != null && playerInfo.underworldMap.Length > 0)
            {
                LoadCavernMap();
            }
            else
            {
                GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + playerInfo.map);
                GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
                map.transform.SetParent(grid.transform, false);
            }
            saver.LoadNPCs();
        }
    }

    private void LoadCavernMap()
    {
        string startString = "A,N2S2E2W2";
        GameObject startMap = LoadCavernMapObj(startString);

        LoadNextCavernMap(startMap, startString, 1);
    }

    private void LoadNextCavernMap(GameObject curMap, string curString, int depth)
    {
        if (depth == 0)
            return;

        List<string> exits = new List<string>();
        string[] possibleExits = {"N", "S", "E", "W"};
        foreach(string ex in possibleExits)
        {
            if (curString.Contains(ex))
                exits.Add(ex);
        }

        foreach(string exit in exits)
        {
            Transform exitDims = curMap.transform.Find(exitDict[exit]);
            //Transform mapBox = curMap.transform.Find("BoundingBox");

            string chunkStr = null;

            //TODO: search Cavern directory for compatible resources
            if (exit == "N" || exit == "S")
                chunkStr = "A,N2S2";

            if (exit == "E" || exit == "W")
                chunkStr = "A,E2W2";
            
            if (chunkStr != null)
            {
                int xAxisDelta = 0, yAxisDelta = 0;

                GameObject chunk = LoadCavernMapObj(chunkStr);
                string oppositeExit = exitDict["!~" + exit];
                Transform chunkExit = chunk.transform.Find(oppositeExit);
                //Transform chunkBox = chunk.transform.Find("BoundingBox");

                if (exit == "N")
                    yAxisDelta = -1;
                
                if (exit == "S")
                    yAxisDelta = 1;

                if (exit == "E")
                    xAxisDelta = -1;

                if (exit == "W")
                    xAxisDelta = 1;

                float x = exitDims.position.x - chunkExit.position.x + xAxisDelta;
                float y = exitDims.position.y - chunkExit.position.y + yAxisDelta;

                chunk.transform.Translate(x, y, 0.0f);
                LoadNextCavernMap(chunk, chunkStr, depth - 1);
            }
        }
    }

    private GameObject LoadCavernMapObj(string prefabName)
    {
        GameObject mapPrefab = Resources.Load<GameObject>("Cavern/" + prefabName);
        GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
        map.transform.SetParent(grid.transform, false);
        return map;
    }
}
