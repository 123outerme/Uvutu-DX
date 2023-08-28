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

class RestoringCavernExit {
    public GameObject chunk;
    public string exitAbbr;
    public string nextChunkStr;
    public int nextChunkDepth;

    public RestoringCavernExit(GameObject restoringChunk, string exit, string nextChunk, int depth)
    {
        chunk = restoringChunk;
        exitAbbr = exit;
        nextChunkStr = nextChunk;
        nextChunkDepth = depth;
    }
}

public class ProceduralGenerator : MonoBehaviour
{
    public GameObject grid;
    public List<GameObject> cavernChunks;

    public GameObject playerInfoParent;
    private PlayerInfo playerInfo;
    
    private Queue<GameObject> chunkQueue = new Queue<GameObject>();
    private GameObject rootChunk = null;
    private int nextDebugId = 1;

    private Dictionary<string, string> exitDict = new Dictionary<string, string>();
    private bool dictLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = playerInfoParent.GetComponent<PlayerInfo>();

        PopulateExitDict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopulateExitDict()
    {
        if (dictLoaded)
            return;
        
        //abbreviations to full names
        exitDict.Add("N", "North");
        exitDict.Add("S", "South");
        exitDict.Add("E", "East");
        exitDict.Add("W", "West");
        //from here onwards: mapping exit on one side to compatible exit on the other
        //requiring 0 degree rotation of map (abbrev -> abbrev)
        exitDict.Add("!N", "S");
        exitDict.Add("!S", "N");
        exitDict.Add("!E", "W");
        exitDict.Add("!W", "E");
        //requiring rotation CW of map (abbrev -> abbrev)
        exitDict.Add("(N", "E");
        exitDict.Add("(S", "W");
        exitDict.Add("(E", "S");
        exitDict.Add("(W", "N");
        //requiring rotation CCW of map (abbrev -> abbrev)
        exitDict.Add(")N", "W");
        exitDict.Add(")S", "E");
        exitDict.Add(")E", "N");
        exitDict.Add(")W", "S");
        //requiring 180 degree rotation of map (abbrev -> abbrev)
        exitDict.Add("@N", "N");
        exitDict.Add("@S", "S");
        exitDict.Add("@E", "E");
        exitDict.Add("@W", "W");
        //from here onwards: mapping each rotation prefix to its inverse rotation operation prefix (prefix -> prefix)
        exitDict.Add("!", "@");
        exitDict.Add("(", ")");
        exitDict.Add(")", "(");
        exitDict.Add("@", "!");
        
        dictLoaded = true;
    }

    public void WriteCavernMapString()
    {
        if (rootChunk == null)
            return;

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
                    //Debug.Log(chunkScript.debugId + ": " + exit + " - " + success + ", " + (childChunk != null));

                    if (success)
                    {
                        if (childChunk != null)
                        {
                            CavernChunk childChunkScript = GetChunkScript(childChunk);

                            if (childChunkScript.timesPrintingVisited <= rootCount)
                            {
                                chunkQueue.Enqueue(childChunk);
                                //Debug.Log(chunk.name.Split(",")[0] + " (" + chunkScript.debugId + ") has " + exit + " exit: " + childChunk.name + " (" + childChunkScript.debugId + ")");
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
                    //*
                    else
                    {
                        //Debug.Log(chunk.name.Split(",")[0] + " (" + chunkScript.debugId + ") has undefined " + exit + " exit");
                        chunkQueue.Enqueue(null);
                        //if exit direction exists but is unconnected (not defined in the exits dictionary), enqueue NULL to record no exit connected
                    }
                    //*/
                }
            }
            else
                cavernMapString += "";
            
            cavernMapString += ",";
        }

        cavernMapString = cavernMapString.TrimEnd(',');
        Debug.Log(cavernMapString);
        playerInfo.underworldMap = cavernMapString;
    }

    public void RestoreCavernMapFromString(string cavernMap)
    {
        Debug.Log(cavernMap);
        PopulateExitDict();
        string[] loadedChunks = cavernMap.Split(",");
        
        GameObject rootChunkPrefab = LoadFindNextChunkPrefab(loadedChunks[0]);
        rootChunk = LoadCavernChunk(rootChunkPrefab);
        CavernChunk rootChunkScript = GetChunkScript(rootChunk);
        string prefix = loadedChunks[0].Substring(0,1);

        float degreesRotation = 0.0f;  //if "!" prefix, then exits line up without rotation
        if (prefix == "(")
            degreesRotation = -90.0f;  //if CCW rotation (match up with opposite of CW rotation) then rotate CCW

        if (prefix == ")")
            degreesRotation = 90.0f;  //if CW rotation (match up with opposite of CW rotation) then rotate CW

        if (prefix == "@")
            degreesRotation = 180.0f;  //if "@" prefix, then to match up equal cardinal directions, 180 degree rotation must occur

        rootChunk.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0.0f, 0.0f, 1.0f), degreesRotation);
        rootChunkScript.rotationPrefix = prefix;
        rootChunkScript.timesPrintingVisited = 1;  //we have visited the root chunk for creating its children now

        int index = 1;
        rootChunkScript.localMaxDepth = 3;
        
        List<string> chunkExits = GetPossibleExitsForChunk(rootChunk, rootChunkScript);

        Queue<RestoringCavernExit> loadQueue = new Queue<RestoringCavernExit>();
        foreach(string exit in chunkExits)
        {
            if (index < loadedChunks.Length && loadedChunks[index] != "")
                loadQueue.Enqueue(new RestoringCavernExit(rootChunk, exit, loadedChunks[index], 1));
            index++;
        }

        while(loadQueue.Count > 0)
        {
            RestoringCavernExit loadExit = loadQueue.Dequeue();
            CavernChunk chunkScript = GetChunkScript(loadExit.chunk);
            //Debug.Log(chunkScript.debugId + ": " + loadExit.chunk.name.Split(",")[0] + " / " + loadExit.exitAbbr + " ; " + loadExit.nextChunkStr + " | " + loadExit.nextChunkDepth);
            chunkScript.timesPrintingVisited = 1;  //we have visited this chunk now for creating its children chunks
            GameObject newChunkPrefab = LoadFindNextChunkPrefab(loadExit.nextChunkStr);

            Transform exitDims = loadExit.chunk.transform.Find(exitDict[loadExit.exitAbbr]);
            prefix = loadExit.nextChunkStr.Substring(0, 1);

            string inversePrefix = exitDict[prefix];  //get the inverse rotation prefix of the applied rotation prefix from the dictionary
            string newExitAbbr = exitDict[inversePrefix + exitDict["!" + loadExit.exitAbbr]];  //take the opposite direction of the current exit and do the inverse of the rotation operation applied to the map

            GameObject newChunk = AttachCavernChunk(new CavernChunkCandidate(newChunkPrefab, newExitAbbr, prefix), loadExit.exitAbbr, loadExit.chunk, GetChunkScript(loadExit.chunk), exitDims);
            CavernChunk newChunkScript = GetChunkScript(newChunk);
            newChunkScript.localMaxDepth += ((loadExit.nextChunkDepth == chunkScript.localMaxDepth) ? 2 : 0);  //update localMaxDepth if it is too low for current depth
            
            chunkExits = GetPossibleExitsForChunk(newChunk, newChunkScript);  //get all unfilled exits for this chunk
            foreach(string exit in chunkExits)
            {
                //if there are still more chunks to load, and the chunk to load is not "undefined", and this algorithm has not visited it yet
                if (index < loadedChunks.Length && loadedChunks[index] != "")
                    loadQueue.Enqueue(new RestoringCavernExit(newChunk, exit, loadedChunks[index], loadExit.nextChunkDepth + 1));
                index++;
            }
        }
    }

    private GameObject LoadFindNextChunkPrefab(string chunkStr)
    {
        foreach(GameObject prefab in cavernChunks)
        {
            if (prefab.name.Split(",")[0] == chunkStr.Substring(1))
                return prefab;
        }

        return null;
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
            CreateCavernChunks();  //start loading next chunk
        }
    }

    public void LoadCavernMap()
    {
        //TODO: pick start state better
        GameObject startPrefab = cavernChunks[0];
        rootChunk = LoadCavernChunk(startPrefab);
        CavernChunk rootChunkScript = GetChunkScript(rootChunk);
        rootChunkScript.rotationPrefix = "!";  //hard-coded to not rotate - TODO when picking a first map make this random as well!

        rootChunkScript.localMaxDepth = 3;  //hard-coded generate 3 more chunks in each direction
        chunkQueue.Enqueue(rootChunk);
        CreateCavernChunks();
    }

    private void CreateCavernChunks()
    {
        //Debug.Log("load; queue size " + chunkQueue.Count);
        while(chunkQueue.Count > 0)
        {
            GameObject curChunk = chunkQueue.Dequeue();

            CavernChunk curChunkScript = GetChunkScript(curChunk);

            if (curChunkScript == null || curChunkScript.depth >= curChunkScript.localMaxDepth)
                return;

            List<string> exits = GetPossibleExitsForChunk(curChunk, curChunkScript);

            foreach(string exit in exits)
            {
                //Debug.Log(curChunk.name + ": exit " + exit + " / " + exitDict[exit]);
                Transform exitDims = curChunk.transform.Find(exitDict[exit]);
                //Transform mapBox = curChunk.transform.Find("BoundingBox");

                List<CavernChunkCandidate> chunksGenned = new List<CavernChunkCandidate>();

                string[] exitPrefixes = {"!", "(", ")", "@"};  //requires {0 degree turn, CW 90 deg turn, CCW 90 deg turn, 180 deg turn}
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
                    CavernChunkCandidate generatedChunk = chunksGenned[Random.Range(0, chunksGenned.Count)];

                    GameObject newChunk = AttachCavernChunk(generatedChunk, exit, curChunk, curChunkScript, exitDims);

                    if (curChunkScript.depth + 1 < curChunkScript.localMaxDepth)
                        chunkQueue.Enqueue(newChunk);

                    //Debug.Log("loaded; queue size " + chunkQueue.Count);
                }
            }
        }
    }

    private GameObject AttachCavernChunk(CavernChunkCandidate generatedChunk, string exit, GameObject curChunk, CavernChunk curChunkScript, Transform existingExit)
    {
        int xAxisDelta = 0, yAxisDelta = 0;

        GameObject newChunk = LoadCavernChunk(generatedChunk.chunkPrefab);
            
        float degreesRotation = 0.0f;  //if "!" prefix, then exits line up without rotation
        if (generatedChunk.dictPrefix == "(")
            degreesRotation = -90.0f;  //exit lines up with 90 degree CW rotation

        if (generatedChunk.dictPrefix == ")")
            degreesRotation = 90.0f;  //exit lines up with 90 degree CCW rotation

        if (generatedChunk.dictPrefix == "@")
            degreesRotation = 180.0f;  //exit lines up with 180 degree rotation

        string oppositeExit = exitDict[generatedChunk.newExitAbbr];  //get full name of opposite abbreviation
        Transform chunkExit = newChunk.transform.Find(oppositeExit);
        //Transform chunkBox = chunk.transform.Find("BoundingBox");

        if (exitDict[generatedChunk.dictPrefix + exit] == "S")
            yAxisDelta = -1;  //lining up a new south exit to an old north one: move 1 unit down
        
        if (exitDict[generatedChunk.dictPrefix + exit] == "N")
            yAxisDelta = 1;  //lining up a new north exit to an old south one: move 1 unit up

        if (exitDict[generatedChunk.dictPrefix + exit] == "W")
            xAxisDelta = -1;  //lining up a new west exit to an old east one: move 1 unit left

        if (exitDict[generatedChunk.dictPrefix + exit] == "E")
            xAxisDelta = 1;  //lining up a new east exit to an old west one: move 1 unit right

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
            GameObject childChunk = null;
            bool foundChild = curChunkScript.exits.TryGetValue(ex, out childChunk); 
            
            if (curChunk.name.Contains(ex) && !foundChild)
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
