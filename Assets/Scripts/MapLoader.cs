using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject playerStatsParent;

    private PlayerStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = playerStatsParent.GetComponent<PlayerStats>();

        LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMap()
    {
        //destroy each child object in the grid to clear space for the new map
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log(stats.map);

        GameObject mapPrefab = Resources.Load<GameObject>(stats.map);
        GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
        map.transform.SetParent(grid.transform, false);
    }
}
