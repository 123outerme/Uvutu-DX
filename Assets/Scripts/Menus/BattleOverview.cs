using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleOverview : MonoBehaviour
{
    public GameObject player;
    public GameObject minion;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    public GameObject healthPanel;
    public GameObject physAtkPanel;
    public GameObject magicAtkPanel;
    public GameObject affinityPanel;
    public GameObject resistancePanel;
    public GameObject speedPanel;
    public GameObject conditionPanel;

    public int selectedCombatant = 0;

    string[] options = {"Player", "Minion", "Enemy1", "Enemy2", "Enemy3"};

    // Start is called before the first frame update
    void Start()
    {
        UpdateAllTabDetails();
        UpdateShownCombatantDetails();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAllTabDetails()
    {
        foreach(string option in options)
        {
            GameObject combatantTabPanel = transform.Find("CombatantListPanel").Find(option + "TabPanel").gameObject;
            GameObject combatantObj = GameObject.Find(option);
            if (combatantObj != null)
            {
                combatantTabPanel.SetActive(true);
                Stats stats = combatantObj.GetComponent<Stats>();
                Image panelImg = combatantTabPanel.GetComponent<Image>();
                if (options[selectedCombatant] == option)
                    panelImg.color = new Color(0.0f, 0.015f, 1.0f, 0.392f);  // 004EFF, 0x64 alpha (100 decimal) - tab's selected color 
                else
                    panelImg.color = new Color (1.0f, 1.0f, 1.0f, 0.392f);  //white, 0x64 alpha (100 decimal) - tab's standard color

                Image icon = combatantTabPanel.transform.Find("CombatantIcon").GetComponent<Image>();
                icon.sprite = stats.combatantStats.sprite;

                TMP_Text nameText = combatantTabPanel.transform.Find("CombatantNameText").GetComponent<TMP_Text>();
                nameText.text = stats.combatantName;

                TMP_Text positionText = combatantTabPanel.transform.Find("CombatantPositionText").GetComponent<TMP_Text>();
                positionText.text = option;
            }
            else
            {
                combatantTabPanel.SetActive(false);
            }
        }
    }

    public void UpdateShownCombatantDetails()
    {
        GameObject[] combatants = {player, minion, enemy1, enemy2, enemy3};
        Stats s = combatants[selectedCombatant].GetComponent<Stats>();
        UpdateHealth(s);
        UpdatePhysAtk(s);
        UpdateMagicAtk(s);
        UpdateAffinity(s);
        UpdateResistance(s);
        UpdateSpeed(s);
    }

    public void SelectCombatant(int combatantIndex)
    {
        selectedCombatant = combatantIndex;
        UpdateAllTabDetails();
        UpdateShownCombatantDetails();
    }

    private void UpdateHealth(Stats s)
    {
        TMP_Text text = healthPanel.transform.Find("StatText").GetComponent<TMP_Text>();
        text.text = s.health + "/" + s.maxHealth;
    }

    private void UpdatePhysAtk(Stats s)
    {
        Transform textObj = physAtkPanel.transform.Find("StatText");
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        text.text = "" + s.physAttack;

        GameObject multiplier = textObj.Find("MultiplierText").gameObject;
        UpdateMultiplier(s.physAttackMultiplier, multiplier);
    }

    private void UpdateMagicAtk(Stats s)
    {
        Transform textObj = magicAtkPanel.transform.Find("StatText");
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        text.text = "" + s.magicAttack;

        GameObject multiplier = textObj.Find("MultiplierText").gameObject;
        UpdateMultiplier(s.magicAttackMultiplier, multiplier);
    }

    private void UpdateAffinity(Stats s)
    {
        Transform textObj = affinityPanel.transform.Find("StatText");
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        text.text = "" + s.affinity;

        GameObject multiplier = textObj.Find("MultiplierText").gameObject;
        UpdateMultiplier(s.affinityMultiplier, multiplier);
    }

    private void UpdateResistance(Stats s)
    {
        Transform textObj = resistancePanel.transform.Find("StatText");
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        text.text = "" + s.resistance;

        GameObject multiplier = textObj.Find("MultiplierText").gameObject;
        UpdateMultiplier(s.resistanceMultiplier, multiplier);
    }

    private void UpdateSpeed(Stats s)
    {
        Transform textObj = speedPanel.transform.Find("StatText");
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        text.text = "" + s.speed;

        GameObject multiplier = textObj.Find("MultiplierText").gameObject;
        UpdateMultiplier(s.speedMultiplier, multiplier);
    }

    private void UpdateMultiplier(float value, GameObject multiplier)
    {
        TMP_Text multiplierText = multiplier.GetComponent<TMP_Text>();
        
        if (Mathf.Abs(value - 1.0f) > 0.00001f)
        {
            multiplierText.text = (value > 0.0f) ? "+" : "-";
            multiplierText.text += Mathf.Floor(Mathf.Abs(value * 100.0f));
            multiplierText.text += "%";
            multiplier.SetActive(true);
        }
        else
            multiplier.SetActive(false);
    }
}
