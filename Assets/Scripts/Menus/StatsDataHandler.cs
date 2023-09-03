using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsDataHandler : MonoBehaviour
{
    public GameObject player;

    public GameObject nameText;
    //public GameObject statsListPanel;
    public GameObject movesPanel;

    private Stats playerStats;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStats = player.GetComponent<Stats>();
        UpdateName();
        //statsListPanel.GetComponent<StatsListPanel>().UpdateStatsList();
        UpdateMovesPanel();
    }

    // Update is called once per frame
    void Update()
    {
        //
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
}
