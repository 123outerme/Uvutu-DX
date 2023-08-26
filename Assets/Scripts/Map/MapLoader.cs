using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CavernChunkGenerated {
    public GameObject chunkPrefab;
    public string newExitAbbr;
    public string dictPrefix;

    public CavernChunkGenerated(GameObject prefab, string exitAbbr, string prefix)
    {
        chunkPrefab = prefab;
        newExitAbbr = exitAbbr;
        dictPrefix = prefix;
    }
}

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject PlayerInfoParent;
    public bool disable = false;

    public List<GameObject> cavernChunks;

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
        //rotation CW of direction (abbrev -> abbrev)
        exitDict.Add("(N", "E");
        exitDict.Add("(S", "W");
        exitDict.Add("(E", "S");
        exitDict.Add("(W", "N");
        //rotation CCW of direction (abbrev -> abbrev)
        exitDict.Add(")N", "W");
        exitDict.Add(")S", "E");
        exitDict.Add(")E", "N");
        exitDict.Add(")W", "S");

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
        GameObject startMap = LoadCavernChunk(cavernChunks[0]);

        LoadNextCavernChunk(startMap, 0, playerInfo.underworldDepth + 3);
    }

    private void LoadNextCavernChunk(GameObject curChunk, int depth, int maxDepth)
    {
        if (depth >= maxDepth)
            return;

        CavernChunk curChunkScript = curChunk.transform.Find("BoundingBox").GetComponent<CavernChunk>();

        List<string> exits = new List<string>();
        string[] possibleExits = {"N", "S", "E", "W"};
        foreach(string ex in possibleExits)
        {
            GameObject tempOutObj;
            if (curChunk.name.Contains(ex) && !curChunkScript.exits.TryGetValue(ex, out tempOutObj))
                exits.Add(ex);
        }

        foreach(string exit in exits)
        {
            Debug.Log(curChunk.name + ": exit " + exit);
            Transform exitDims = curChunk.transform.Find(exitDict[exit]);
            //Transform mapBox = curChunk.transform.Find("BoundingBox");

            List<CavernChunkGenerated> chunkPrefabs = new List<CavernChunkGenerated>();

            string[] exitPrefixes = {"!", "(", ")"};
            foreach(string prefix in exitPrefixes)
            {
                string opEx = exitDict[prefix + exit];
                foreach(GameObject prefab in cavernChunks)
                {
                    if (prefab.name.Contains(opEx) && prefab.name != curChunk.name.Split("(Clone)")[0])
                    {
                        //if the prefab contains the exit we need to pair and it isn't a prefab version of the current chunk 
                        chunkPrefabs.Add(new CavernChunkGenerated(prefab, opEx, prefix));
                        break;
                    }
                }
            }
            
            if (chunkPrefabs.Count > 0)
            {
                //TODO: pick a prefab better
                CavernChunkGenerated generatedChunk = chunkPrefabs[Random.Range(0, chunkPrefabs.Count - 1)];

                int xAxisDelta = 0, yAxisDelta = 0;

                GameObject newChunk = LoadCavernChunk(generatedChunk.chunkPrefab);
                
                float degreesRotation = 0.0f;
                if (generatedChunk.dictPrefix == "(")
                    degreesRotation = -90.0f;

                if (generatedChunk.dictPrefix == ")")
                    degreesRotation = 90.0f;

                string oppositeExit = exitDict[generatedChunk.newExitAbbr];  //get full name of opposite abbreviation
                Transform chunkExit = newChunk.transform.Find(oppositeExit);
                //Transform chunkBox = chunk.transform.Find("BoundingBox");

                if (exitDict[generatedChunk.dictPrefix + exit] == "S")
                    yAxisDelta = -1;
                
                if (exitDict[generatedChunk.dictPrefix + exit] == "N")
                    yAxisDelta = 1;

                if (exitDict[generatedChunk.dictPrefix + exit] == "W")
                    xAxisDelta = -1;

                if (exitDict[generatedChunk.dictPrefix + exit] == "E")
                    xAxisDelta = 1;

                float x = exitDims.position.x - chunkExit.position.x;
                float y = exitDims.position.y - chunkExit.position.y;

                newChunk.transform.Translate(x, y, 0.0f);
                newChunk.transform.RotateAround(chunkExit.position, new Vector3(0.0f, 0.0f, 1.0f), degreesRotation + curChunk.transform.rotation.eulerAngles.z);
                newChunk.transform.Translate(xAxisDelta, yAxisDelta, 0.0f);
                
                curChunkScript.exits.Add(exit, newChunk);
                
                CavernChunk newChunkScript = newChunk.transform.Find("BoundingBox").GetComponent<CavernChunk>();
                newChunkScript.depth = depth + 1;
                newChunkScript.exits.Add(generatedChunk.newExitAbbr, curChunk);

                LoadNextCavernChunk(newChunk, depth + 1, maxDepth);
            }
        }
    }

    private GameObject LoadCavernChunk(GameObject chunkPrefab)
    {
        GameObject chunk = GameObject.Instantiate(chunkPrefab) as GameObject;
        chunk.transform.SetParent(grid.transform, false);
        return chunk;
    }
}
