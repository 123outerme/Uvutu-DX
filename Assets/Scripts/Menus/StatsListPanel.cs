using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsListPanel : MonoBehaviour
{   
    public GameObject player;

    public GameObject levelText;
    public GameObject healthText;
    public GameObject expText;
    public GameObject physAtkPanel;
    public GameObject magicAtkPanel;
    public GameObject affinityPanel;
    public GameObject resistancePanel;
    public GameObject speedPanel;
    public GameObject statPtsText;

    private Stats playerStats;
    private PlayerInfo playerInfo;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = player.GetComponent<Stats>();
        playerInfo = player.GetComponent<PlayerInfo>();
        UpdateStatsList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStatsList()
    {
        UpdateLevel();
        UpdateHealth();
        UpdateExp();
        UpdatePhysAtk();
        UpdateMagicAtk();
        UpdateAffinity();
        UpdateResistance();
        UpdateSpeed();
        UpdateStatPts();
    }

    public void UpdateLevel()
    {
        TMP_Text text = levelText.GetComponent<TMP_Text>();
        text.text = "" + playerStats.level;
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
        text.text = AddCommasToInt(playerStats.exp) + "/" + AddCommasToInt(playerStats.PlayerCalculateNeededExp());
    }

    public void UpdatePhysAtk()
    {
        TMP_Text text = physAtkPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.physAttack;
        UpdateIncreaseStatButton(physAtkPanel);
        UpdateDecreaseStatButton(physAtkPanel);
    }

    public void UpdateMagicAtk()
    {
        TMP_Text text = magicAtkPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.magicAttack;
        UpdateIncreaseStatButton(magicAtkPanel);
        UpdateDecreaseStatButton(magicAtkPanel);
    }

    public void UpdateAffinity()
    {
        TMP_Text text = affinityPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.affinity;
        UpdateIncreaseStatButton(affinityPanel);
        UpdateDecreaseStatButton(affinityPanel);
    }

    public void UpdateResistance()
    {
        TMP_Text text = resistancePanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.resistance;
        UpdateIncreaseStatButton(resistancePanel);
        UpdateDecreaseStatButton(resistancePanel);
    }

    public void UpdateSpeed()
    {
        TMP_Text text = speedPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStats.speed;
        UpdateIncreaseStatButton(speedPanel);
        UpdateDecreaseStatButton(speedPanel);
    }

    private void UpdateIncreaseStatButton(GameObject panel)
    {
        Button button = panel.transform.Find("StatIncreaseButton").gameObject.GetComponent<Button>();
        button.interactable = (playerInfo.statPoints > 0);
    }

    private void UpdateDecreaseStatButton(GameObject panel)
    {
        Button button = panel.transform.Find("StatDecreaseButton").gameObject.GetComponent<Button>();
        button.interactable = (playerInfo.statPtPool > 0);
    }

    public void UpdateStatPts()
    {
        TMP_Text text = statPtsText.GetComponent<TMP_Text>();
        text.text = "" + playerInfo.statPoints;
    }

    private string AddCommasToInt(int number)
    {
        string numString = "";
        string convertNum = "" + number;

        for(int i = convertNum.Length - 1; i >= 0; i--)
        {
            numString = convertNum[i] + numString;
            if ((convertNum.Length - i) % 3 == 0 && i != 0)
                numString = "," + numString;
        }

        return numString;
    }

    public void IncreasePhysAtk()
    {
        playerInfo.statPoints--;
        playerStats.physAttack++;
        UpdatePhysAtk();
        UpdateStatPts();
    }

    public void IncreaseMagicAttack()
    {
        playerInfo.statPoints--;
        playerStats.magicAttack++;
        UpdateMagicAtk();
        UpdateStatPts();
    }

    public void IncreaseAffinity()
    {
        playerInfo.statPoints--;
        playerStats.affinity++;
        UpdateAffinity();
        UpdateStatPts();
    }

    public void IncreaseResistance()
    {
        playerInfo.statPoints--;
        playerStats.resistance++;
        UpdateResistance();
        UpdateStatPts();
    }

    public void IncreaseSpeed()
    {
        playerInfo.statPoints--;
        playerStats.speed++;
        UpdateSpeed();
        UpdateStatPts();
    }
}
