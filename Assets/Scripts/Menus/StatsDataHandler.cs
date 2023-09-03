using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsDataHandler : MonoBehaviour
{
    public GameObject player;

    public GameObject nameText;
    public GameObject statsListPanel;
    public GameObject saveChangesButton;
    public GameObject movesPanel;

    private Stats playerStats;
    private StatsListPanel statsListScript = null;
    private Button saveChangesButtonScript = null;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStats = player.GetComponent<Stats>();
        statsListScript = statsListPanel.GetComponent<StatsListPanel>();
        UpdateName();
        UpdateMovesPanel();
        UpdateSaveStatsButton();
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    private void GetSaveButtonScript()
    {
        if (saveChangesButtonScript == null)
            saveChangesButtonScript = saveChangesButton.GetComponent<Button>();
    }

    public void UpdateName()
    {
        TMP_Text text = nameText.GetComponent<TMP_Text>();
        text.text = playerStats.combatantName;
    }

    public void UpdateMovesPanel()
    {
        for(int i = 0; i < playerStats.moveset.Length; i++)
        {
            TMP_Text moveNameText = movesPanel.transform.Find("PanelMove" + (i+1) + "/MoveText").GetComponent<TMP_Text>();
            moveNameText.text = playerStats.moveset[i];
        }
    }

    public void UpdateSaveStatsButton()
    {
        GetSaveButtonScript();
        saveChangesButtonScript.interactable = statsListScript.HaveStatsChanged();
    }
}
