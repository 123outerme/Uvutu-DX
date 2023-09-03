using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject mainPausePanel;
    public GameObject statsPanel;
    public GameObject statsListPanel;
    public GameObject inventoryPanel;
    public GameObject questsPanel;
    public GameObject player;

    private InventoryPanel invPanelScript;
    private PlayerInfo playerInfo;
    private StatsListPanel statsListScript;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private SaveHandler saver;
    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
        playerInfo = player.GetComponent<PlayerInfo>();
        invPanelScript = inventoryPanel.GetComponent<InventoryPanel>();
        statsListScript = statsListPanel.GetComponent<StatsListPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        SaveGame();
        scenesToLoad.Add(SceneManager.LoadSceneAsync(saver.GetSceneToLoad()));
    }

    public void ShowStats(bool setting)
    {
        statsPanel.SetActive(setting);
    }

    public void ShowInventory(bool setting)
    {   
        invPanelScript.disableUseButtonOverride = playerInfo.inBattle;  //disable usage of items in the pause menu while in battle (they can be used as an action instead)
        inventoryPanel.SetActive(setting);
    }

    public void ShowQuests(bool setting)
    {
        questsPanel.SetActive(setting);
    }

    public void OpenSettings()
    {
        //TODO
    }

    public void SaveGame()
    {
        statsListScript.CancelStatChanges();
        saver.Save();
    }

    public void ExitGame()
    {
        SaveGame();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("MainMenu"));
    }

    public void UseItem(InventorySlot slot)
    {
        //Use item in pause menu
    }
}
