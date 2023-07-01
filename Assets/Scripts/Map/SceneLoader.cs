using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public GameObject player;
    public SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPauseMenu()
    {
        SavePlayerData();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("PauseMenu"));
    }

    void SavePlayerData()
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.position = player.transform.position;
        saver.Save();
    }
}
