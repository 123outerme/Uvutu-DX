using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject navMesh;
    public GameObject playerInfoParent;
    public bool disable = false;

    public ProceduralGenerator proceduralGenerator;

    private PlayerInfo playerInfo;
    private SaveHandler saver;
    private NavMeshSurface navSurface2D;
    private bool buildMeshFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = playerInfoParent.GetComponent<PlayerInfo>();
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();

        LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildMeshFlag)
            BuildNavMesh();
    }

    public void LoadMap()
    {
        if (!disable)
        {
            //Debug.Log("save before loading new map");
            saver.Save();
            
            //destroy each child object in the grid to clear space for the new map
            foreach(Transform child in grid.transform)
                Destroy(child.gameObject);

            //Debug.Log(playerInfo.map);

            if (playerInfo.underworldMap != null && playerInfo.underworldMap.Length > 0)
            {
                proceduralGenerator.LoadCavernBaseMap();
                if (playerInfo.underworldMap == "new")
                    proceduralGenerator.LoadCavernMap();
                else
                    proceduralGenerator.RestoreCavernMapFromString(playerInfo.underworldMap);
            }
            else
            {
                GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + playerInfo.map);
                GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
                map.transform.SetParent(grid.transform, false);
            }
            saver.LoadNPCs();
            PrepareNewNavMesh();
        }
    }

    public void PrepareNewNavMesh()
    {
        //reset nav mesh and rebuild
        if (navSurface2D == null)
        {
            navSurface2D = navMesh.GetComponent<NavMeshSurface>();
            navSurface2D.hideEditorLogs = true;
        }
        
        navSurface2D.RemoveData();
        NavMesh.RemoveAllNavMeshData();
        buildMeshFlag = true;
    }

    private void BuildNavMesh()
    {
        navSurface2D.BuildNavMeshAsync();
        buildMeshFlag = false;
    }
}
