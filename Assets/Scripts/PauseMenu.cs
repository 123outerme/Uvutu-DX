using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject mainPauseScreen;
    public GameObject statsScreen;
    public GameObject inventoryScreen;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private SaveHandler saver;
    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Overworld"));
    }

    public void ShowStats(bool setting)
    {
        statsScreen.SetActive(setting);
    }

    public void ShowInventory()
    {
        //TODO
    }

    public void OpenSettings()
    {
        //TODO
    }

    public void SaveGame()
    {
        //TODO
        saver.Save();
    }

    public void ExitGame()
    {
        SaveGame();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("MainMenu"));
    }
}
