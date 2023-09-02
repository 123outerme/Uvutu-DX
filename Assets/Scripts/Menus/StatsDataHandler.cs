using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsDataHandler : MonoBehaviour
{
    public GameObject player;

    public GameObject nameText;
    public GameObject healthText;
    public GameObject expText;
    public GameObject physAtkPanel;
    public GameObject magicAtkPanel;
    public GameObject affinityPanel;
    public GameObject resistancePanel;
    public GameObject speedPanel;
    public GameObject statPtsText;
    public GameObject movesPanel;

    private Stats playerStats;
    private PlayerInfo playerInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStats = player.GetComponent<Stats>();
        playerInfo = player.GetComponent<PlayerInfo>();
        UpdateStatPts();
        UpdateName();
        UpdateHealth();
        UpdateExp();
        UpdatePhysAtk();
        UpdateMagicAtk();
        UpdateAffinity();
        UpdateResistance();
        UpdateSpeed();
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

    public void UpdateHealth()
    {
        TMP_Text text = healthText.GetComponent<TMP_Text>();
        text.text = playerStats.health + "/" + playerStats.maxHealth;
    }

    public void UpdateExp()
    {
        //TODO: convert this to a filling Exp bar
        TMP_Text text = expText.GetComponent<TMP_Text>();
        text.text = "" + playerStats.exp;
    }

    public void UpdatePhysAtk()
    {
        TMP_Text text = physAtkPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.physAttack;
    }

    public void UpdateMagicAtk()
    {
        TMP_Text text = magicAtkPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.magicAttack;
    }

    public void UpdateAffinity()
    {
        TMP_Text text = affinityPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.affinity;
    }

    public void UpdateResistance()
    {
        TMP_Text text = resistancePanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.resistance;
    }

    public void UpdateSpeed()
    {
        TMP_Text text = speedPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.speed;
    }

    public void UpdateStatPts()
    {
        TMP_Text text = statPtsText.GetComponent<TMP_Text>();
        text.text = "" + playerInfo.statPoints;

        physAtkPanel.transform.Find("StatIncreaseButton").GetComponent<Button>().interactable = (playerInfo.statPoints > 0);
        magicAtkPanel.transform.Find("StatIncreaseButton").GetComponent<Button>().interactable = (playerInfo.statPoints > 0);
        affinityPanel.transform.Find("StatIncreaseButton").GetComponent<Button>().interactable = (playerInfo.statPoints > 0);
        resistancePanel.transform.Find("StatIncreaseButton").GetComponent<Button>().interactable = (playerInfo.statPoints > 0);
        speedPanel.transform.Find("StatIncreaseButton").GetComponent<Button>().interactable = (playerInfo.statPoints > 0);
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
