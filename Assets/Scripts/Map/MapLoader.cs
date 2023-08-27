using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CavernChunkCandidate {
    public GameObject chunkPrefab;
    public string newExitAbbr;
    public string dictPrefix;

    public CavernChunkCandidate(GameObject prefab, string exitAbbr, string prefix)
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
    private Queue<GameObject> chunkQueue = new Queue<GameObject>();
    private GameObject rootChunk = null;
    private int nextDebugId = 1;

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
        //180 rotation of direction (abbrev -> abbrev)
        exitDict.Add("@N", "W");
        exitDict.Add("@S", "E");
        exitDict.Add("@E", "N");
        exitDict.Add("@W", "S");

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
                if (playerInfo.underworldMap == "new")
                {
                    //TODO: pick start state better
                    GameObject startPrefab = cavernChunks[0];
                    LoadCavernMap(startPrefab);
                }
                else
                {
                    RestoreCavernMapFromString();
                }
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

    public void RestoreCavernMapFromString()
    {
        //TODO traverse playerInfo.underworldMap string and convert it back to map chunks
    }

    public void WriteCavernMapString()
    {
        chunkQueue.Clear();
        chunkQueue.Enqueue(rootChunk);
        string cavernMapString = "";

        int rootCount = GetChunkScript(rootChunk).timesPrintingVisited;

        while(chunkQueue.Count > 0)
        {
            //Debug.Log("do while: " + chunkQueue.Count);
            GameObject chunk = chunkQueue.Dequeue();
            CavernChunk chunkScript = GetChunkScript(chunk);
            if (chunk != null && chunkScript != null)
            {
                chunkScript.timesPrintingVisited = rootCount + 1;
                cavernMapString += chunkScript.rotationPrefix + chunk.name.Split(",")[0];

                string[] possibleExits = {"N", "S", "E", "W"};
                foreach(string exit in possibleExits)
                {
                    GameObject childChunk = null;
                    bool success = chunkScript.exits.TryGetValue(exit, out childChunk);

                    if (success)
                    {
                        if (childChunk != null)
                        {
                            CavernChunk childChunkScript = GetChunkScript(childChunk);

                            if (childChunkScript.timesPrintingVisited <= rootCount)
                            {
                                chunkQueue.Enqueue(childChunk);
                                Debug.Log(chunk.name.Split(",")[0] + " (" + chunkScript.debugId + ") has " + exit + " exit: " + childChunk.name + " (" + childChunkScript.debugId + ")");
                            }
                            /*
                            else
                                Debug.Log("already visited " + childChunk.name + " (" + childChunkScript.debugId + ") to the " + exit);
                            //else, this has already been visited for this string generation  */
                        }
                        /*
                        else
                            Debug.Log(chunk.name.Split(",")[0] + " (" + chunkScript.debugId + ") has no " + exit + " exit");    
                        //else, do not enqueue anything else (this chunk does not have this exit direction!) */
                    }
                    /*
                    else
                        Debug.Log(chunk.name.Split(",")[0] + " (" + chunkScript.debugId + ") has undefined " + exit + " exit");
                    //if exit direction exists but is unconnected (not defined in the exits dictionary), enqueue NULL to record no exit connected */
                }
            }

            if (chunkQueue.Count > 0)
                cavernMapString += ",";
        }
        Debug.Log(cavernMapString);

        playerInfo.underworldMap = cavernMapString;
    }

    public bool ShouldLoadMoreCavern(int depth, int localMaxDepth)
    {
        return depth + 2 > localMaxDepth;  //if there are less than 2 more chunks until the current "frontier"
    }

    public void LoadMoreCavern(GameObject chunk)
    {
        Debug.Log("load more cavern / " + chunk.name);
        CavernChunk chunkScript = GetChunkScript(chunk);

        string[] exits = {"N", "S", "E", "W"};
        //NOTE: if ShouldLoadMoreCavern(int) changes, the logic here to put the chunks that are at the end of the "tree" should change 
        foreach(string exit in exits)
        {
            GameObject childChunk = null;
            bool found = chunkScript.exits.TryGetValue(exit, out childChunk);
            if (found && childChunk != null)
            {
                GetChunkScript(childChunk).localMaxDepth = chunkScript.localMaxDepth + 2;  //generate 2 more chunks from the current local maximum
                chunkQueue.Enqueue(childChunk);
            }
        }

        if (chunkQueue.Count > 0)
        {
            chunkScript.localMaxDepth += 2;  //if any children were found, increase the local max depth to prevent processing happening again
            LoadNextCavernChunk();  //start loading next chunk
            WriteCavernMapString();  //export the representative string after generation completes
        }
    }

    private void LoadCavernMap(GameObject startPrefab)
    {
        rootChunk = LoadCavernChunk(startPrefab);
        CavernChunk rootChunkScript = GetChunkScript(rootChunk);
        rootChunkScript.rotationPrefix = "!";  //hard-coded to not rotate - TODO when picking a first map make this random as well!

        rootChunkScript.localMaxDepth = playerInfo.underworldDepth + 3;
        chunkQueue.Enqueue(rootChunk);
        LoadNextCavernChunk();
        WriteCavernMapString();
    }

    private void LoadNextCavernChunk()
    {
        //Debug.Log("load; queue size " + chunkQueue.Count);

        if (chunkQueue.Count == 0)
            return;

        GameObject curChunk = chunkQueue.Dequeue();

        CavernChunk curChunkScript = GetChunkScript(curChunk);

        if (curChunkScript == null || curChunkScript.depth >= curChunkScript.localMaxDepth)
            return;

        List<string> exits = GetPossibleExitsForChunk(curChunk, curChunkScript);

        foreach(string exit in exits)
        {
            //Debug.Log(curChunk.name + ": exit " + exit);
            Transform exitDims = curChunk.transform.Find(exitDict[exit]);
            //Transform mapBox = curChunk.transform.Find("BoundingBox");

            List<CavernChunkCandidate> chunksGenned = new List<CavernChunkCandidate>();

            string[] exitPrefixes = {"!", "(", ")", "@"};
            foreach(string prefix in exitPrefixes)
            {
                string opEx = exitDict[prefix + exit];
                foreach(GameObject prefab in cavernChunks)
                {
                    if (prefab.name.Contains(opEx) && prefab.name != curChunk.name.Split("(Clone)")[0])
                    {
                        //if the prefab contains the exit we need to pair and it isn't a prefab version of the current chunk 
                        chunksGenned.Add(new CavernChunkCandidate(prefab, opEx, prefix));
                        break;
                    }
                }
            }
            
            if (chunksGenned.Count > 0)
            {
                //TODO: pick a prefab better
                CavernChunkCandidate generatedChunk = chunksGenned[Random.Range(0, chunksGenned.Count - 1)];
                /*
                foreach(CavernChunkCandidate prefab in chunksGenned)
                {
                    //if there is already data on what map this should be and it is in the list of chunks that can be used
                    if ()
                        generatedChunk = prefab;  //set the chunk to be what is saved
                }
                //*/
                GameObject newChunk = AttachCavernChunk(generatedChunk, exit, curChunk, curChunkScript, exitDims);

                if (curChunkScript.depth + 1 < curChunkScript.localMaxDepth)
                    chunkQueue.Enqueue(newChunk);

                //Debug.Log("loaded; queue size " + chunkQueue.Count);
            }
        }

        LoadNextCavernChunk();
    }

    private GameObject AttachCavernChunk(CavernChunkCandidate generatedChunk, string exit, GameObject curChunk, CavernChunk curChunkScript, Transform existingExit)
    {
        int xAxisDelta = 0, yAxisDelta = 0;

        GameObject newChunk = LoadCavernChunk(generatedChunk.chunkPrefab);
            
        float degreesRotation = 0.0f;
        if (generatedChunk.dictPrefix == "(")
            degreesRotation = -90.0f;

        if (generatedChunk.dictPrefix == ")")
            degreesRotation = 90.0f;

        if (generatedChunk.dictPrefix == "@")
            degreesRotation = 180.0f;

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

        float x = existingExit.position.x - chunkExit.position.x;
        float y = existingExit.position.y - chunkExit.position.y;

        newChunk.transform.Translate(x, y, 0.0f);  //move exits to be on top of each other
        newChunk.transform.RotateAround(chunkExit.position, new Vector3(0.0f, 0.0f, 1.0f), degreesRotation + curChunk.transform.rotation.eulerAngles.z);  //rotate to have new chunk's exit go "outwards" from old chunk's
        newChunk.transform.Translate(xAxisDelta, yAxisDelta, 0.0f);  //move chunk exit so that there is no gap between 
            
        curChunkScript.exits.TryAdd(exit, newChunk);
        curChunkScript.PresentDictionary();
            
        CavernChunk newChunkScript = GetChunkScript(newChunk);
        newChunkScript.depth = curChunkScript.depth + 1;
        newChunkScript.exits.TryAdd(generatedChunk.newExitAbbr, curChunk);
        newChunkScript.PresentDictionary();
        newChunkScript.debugId = nextDebugId;
        nextDebugId++;

        newChunkScript.rotationPrefix = generatedChunk.dictPrefix;
        newChunkScript.localMaxDepth = curChunkScript.localMaxDepth;  //copy local max depth from parent to child

        return newChunk;
    }

    private GameObject LoadCavernChunk(GameObject chunkPrefab)
    {
        GameObject chunk = GameObject.Instantiate(chunkPrefab) as GameObject;
        chunk.transform.SetParent(grid.transform, false);
        return chunk;
    }

    private List<string> GetPossibleExitsForChunk(GameObject curChunk, CavernChunk curChunkScript)
    {
        List<string> exits = new List<string>();
        string[] possibleExits = {"N", "S", "E", "W"};
        foreach(string ex in possibleExits)
        {
            GameObject childChunk;
            bool foundChild = curChunkScript.exits.TryGetValue(ex, out childChunk); 
            
            if (curChunk.name.Contains(ex) && (!foundChild || childChunk == null))
                exits.Add(ex);
        }

        return exits;
    }
    
    CavernChunk GetChunkScript(GameObject chunk)
    {
        if (chunk == null)
            return null;

        return chunk.transform.Find("BoundingBox").GetComponent<CavernChunk>();
    }
}
