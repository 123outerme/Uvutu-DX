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

    private Stats playerStats = null;
    private PlayerInfo playerInfo = null;
    private StatLine playerStatLineCopy = null;

    // Start is called before the first frame update
    void Start()
    {
        GetPlayerData();
        GetStatLineCopy();
        UpdateStatsList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetPlayerData()
    {
        if (playerStats == null)
            playerStats = player.GetComponent<Stats>();

        if (playerInfo == null)
            playerInfo = player.GetComponent<PlayerInfo>();
    }

    public void GetStatLineCopy()
    {
        GetPlayerData();
        if (playerStatLineCopy == null && playerStats != null)
            SetStatLineCopy(playerStats.statLine.Copy());
    }

    public void SetStatLineCopy(StatLine s)
    {
        Debug.Log(s.magicAttack);
        playerStatLineCopy = s;
        UpdateStatsList();
    }

    public void UpdateStatsList()
    {
        GetPlayerData();

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
        text.text = playerStats.health + "/" + playerStats.statLine.maxHealth;
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
        text.text = "" + playerStatLineCopy.physAttack;
        UpdateIncreaseStatButton(physAtkPanel);
        UpdateDecreaseStatButton(physAtkPanel, playerStats.statLine.physAttack, playerStatLineCopy.physAttack);
    }

    public void UpdateMagicAtk()
    {
        TMP_Text text = magicAtkPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStatLineCopy.magicAttack;
        UpdateIncreaseStatButton(magicAtkPanel);
        UpdateDecreaseStatButton(magicAtkPanel, playerStats.statLine.magicAttack, playerStatLineCopy.magicAttack);
    }

    public void UpdateAffinity()
    {
        TMP_Text text = affinityPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStatLineCopy.affinity;
        UpdateIncreaseStatButton(affinityPanel);
        UpdateDecreaseStatButton(affinityPanel, playerStats.statLine.affinity, playerStatLineCopy.affinity);
    }

    public void UpdateResistance()
    {
        TMP_Text text = resistancePanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStatLineCopy.resistance;
        UpdateIncreaseStatButton(resistancePanel);
        UpdateDecreaseStatButton(resistancePanel, playerStats.statLine.resistance, playerStatLineCopy.resistance);
    }

    public void UpdateSpeed()
    {
        TMP_Text text = speedPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = "" + playerStatLineCopy.speed;
        UpdateIncreaseStatButton(speedPanel);
        UpdateDecreaseStatButton(speedPanel, playerStats.statLine.speed, playerStatLineCopy.speed);
    }

    private void UpdateIncreaseStatButton(GameObject panel)
    {
        Button button = panel.transform.Find("StatIncreaseButton").gameObject.GetComponent<Button>();
        button.interactable = (playerInfo.statPoints > 0);
    }

    private void UpdateDecreaseStatButton(GameObject panel, int prevStat, int curStat)
    {
        Button button = panel.transform.Find("StatDecreaseButton").gameObject.GetComponent<Button>();
        button.interactable = (playerInfo.statPtPool > 0 && curStat > prevStat);
    }

    public void UpdateStatPts()
    {
        TMP_Text text = statPtsText.GetComponent<TMP_Text>();
        text.text = "" + playerInfo.statPoints;
    }

    public static string AddCommasToInt(int number)
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

    public void IncreasePhysAtk(bool increase)
    {
        if (increase)
        {
            playerInfo.statPoints--;
            playerStatLineCopy.physAttack++;
        }
        else
        {
            playerInfo.statPoints++;
            playerStatLineCopy.physAttack--;
        }
        UpdatePhysAtk();
        UpdateStatPts();
    }

    public void IncreaseMagicAttack(bool increase)
    {
        if (increase)
        {
            playerInfo.statPoints--;
            playerStatLineCopy.magicAttack++;
        }
        else
        {
            playerInfo.statPoints++;
            playerStatLineCopy.magicAttack--;
        }
        UpdateMagicAtk();
        UpdateStatPts();
    }

    public void IncreaseAffinity(bool increase)
    {
        if (increase)
        {
            playerInfo.statPoints--;
            playerStatLineCopy.affinity++;
        }
        else
        {
            playerInfo.statPoints++;
            playerStatLineCopy.affinity--;
        }
        UpdateAffinity();
        UpdateStatPts();
    }

    public void IncreaseResistance(bool increase)
    {
        if (increase)
        {
            playerInfo.statPoints--;
            playerStatLineCopy.resistance++;
        }
        else
        {
            playerInfo.statPoints++;
            playerStatLineCopy.resistance--;
        }
        UpdateResistance();
        UpdateStatPts();
    }

    public void IncreaseSpeed(bool increase)
    {
        if (increase)
        {
            playerInfo.statPoints--;
            playerStatLineCopy.speed++;
        }
        else
        {
            playerInfo.statPoints++;
            playerStatLineCopy.speed--;
        }
        UpdateSpeed();
        UpdateStatPts();
    }

    public void ConfirmStats()
    {
        if (playerStatLineCopy != null)
        {
            playerStats.statLine = playerStatLineCopy;
            playerInfo.statPtPool = playerInfo.statPoints;
        }
    }

    public void CancelStatChanges()
    {
        if (playerInfo != null)
            playerInfo.statPoints = playerInfo.statPtPool;
    }

    public bool HaveStatsChanged()
    {
        if (playerStatLineCopy == null)
            return false;

        return !(playerStatLineCopy.DeepEquals(playerStats.statLine));
    }
}
