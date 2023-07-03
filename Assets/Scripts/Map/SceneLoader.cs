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
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        SavePlayerData();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Battle"));
    }

    public void ResumeGame()
    {
        SavePlayerData();
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        string loadScene = location.scene;

        if (location.inBattle)
            loadScene = "Battle";

        scenesToLoad.Add(SceneManager.LoadSceneAsync(loadScene));  //resume from the proper scene
    }

    void SavePlayerData()
    {
        PlayerLocation location = player.GetComponent<PlayerLocation>();
        if (location.usePosition)  //don't overwrite position if the scene does not use saved world position
            location.position = player.transform.position;
        saver.Save();
    }
}
