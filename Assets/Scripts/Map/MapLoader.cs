using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject playerStatsParent;
    public bool disable = false;

    private PlayerStats stats;

    private SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        stats = playerStatsParent.GetComponent<PlayerStats>();
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();

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
            Debug.Log("save before loading new map");
            saver.Save();  //TODO broken
            
            //destroy each child object in the grid to clear space for the new map
            foreach(Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }

            Debug.Log(stats.map);

            GameObject mapPrefab = Resources.Load<GameObject>(stats.map);
            GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
            map.transform.SetParent(grid.transform, false);

            saver.LoadNPCs();
        }
    }
}
