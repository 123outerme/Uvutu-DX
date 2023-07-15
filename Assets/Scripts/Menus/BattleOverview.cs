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

    public GameObject detailsPanel;

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
        GameObject detailsPanel = transform.Find("DetailsPanel").gameObject;
        //TODO
    }

    public void SelectCombatant(int combatantIndex)
    {
        selectedCombatant = combatantIndex;
        UpdateAllTabDetails();
        UpdateShownCombatantDetails();
    }
}
