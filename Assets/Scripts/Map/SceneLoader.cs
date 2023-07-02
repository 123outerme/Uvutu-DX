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

    public void LoadBattle()
    {
        SavePlayerData();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Battle"));
    }

    public void ResumeGame()
    {
        SavePlayerData();
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        scenesToLoad.Add(SceneManager.LoadSceneAsync(location.scene));  //resume from the proper scene
    }

    void SavePlayerData()
    {
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        location.position = player.transform.position;
        saver.Save();
    }
}
