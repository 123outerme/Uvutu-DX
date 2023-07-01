using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject mainPausePanel;
    public GameObject statsPanel;
    public GameObject inventoryPanel;
    public GameObject questsPanel;

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
        statsPanel.SetActive(setting);
    }

    public void ShowInventory()
    {
        //TODO
    }

    public void ShowQuests()
    {
        //TODO
    }

    public void OpenSettings()
    {
        //TODO
    }

    public void SaveGame()
    {
        saver.Save();
    }

    public void ExitGame()
    {
        SaveGame();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("MainMenu"));
    }
}
