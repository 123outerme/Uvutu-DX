using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject grid;
    public GameObject playerLocationParent;
    public bool disable = false;

    private PlayerLocation location;

    private SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        location = playerLocationParent.GetComponent<PlayerLocation>();
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
            saver.Save();  //TODO broken?
            
            //destroy each child object in the grid to clear space for the new map
            foreach(Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }

            Debug.Log(location.map);

            GameObject mapPrefab = Resources.Load<GameObject>(location.map);
            GameObject map = GameObject.Instantiate(mapPrefab) as GameObject;
            map.transform.SetParent(grid.transform, false);

            saver.LoadNPCs();
        }
    }
}
