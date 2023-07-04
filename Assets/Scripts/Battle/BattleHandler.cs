using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHandler : MonoBehaviour
{
    public GameObject player;
    public GameObject minion;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    public GameObject playerHealthPanel;
    public GameObject minionHealthPanel;
    public GameObject enemy1HealthPanel;
    public GameObject enemy2HealthPanel;
    public GameObject enemy3HealthPanel;

    private string[] availableEnemyTypes;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: decide upon selection of enemy types
        List<string> enemyOptions = new List<string> {"Combatants/Ant", "Combatants/Rat"};
        //randomly generate 1-3 enemies to fight
        //TODO: weighted randomness, random level within appropriate range for location
        int enemy1Pick = Mathf.RoundToInt(Random.Range(0, enemyOptions.Count));
        int enemy2Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
        int enemy3Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
        //Debug.Log("" + enemyOptions.Count + " , " + enemy1Pick + " , " + enemy2Pick + " , " + enemy3Pick);

        bool startFromLoad = false;
        Stats enemy1Stats = enemy1.GetComponent<Stats>();
        Debug.Log(enemy1Stats.combatantStats.combatantName);
        Stats enemy2Stats = enemy2.GetComponent<Stats>();
        Stats enemy3Stats = enemy3.GetComponent<Stats>();

        if (enemy1Stats.combatantStats.combatantName != "MissingNo.")
            startFromLoad = true;

        //*
        if (!startFromLoad)
        {
            enemy1Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy1Pick]);
            enemy1Stats.UpdateStats();
            
            if (enemy2Pick >= 0)
            {
                enemy2Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy2Pick]);
                enemy2Stats.UpdateStats();
            }

            if (enemy3Pick >= 0)
            {
                enemy3Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy3Pick]);
                enemy3Stats.UpdateStats();
            }
        }
        else
        {
            //set enabled flag for below, pick ID does not matter for enemy option after this
            if (enemy2Stats.combatantStats.combatantName == "MissingNo.")
                enemy2Pick = -1;
            else
                enemy2Pick = 0;
            if (enemy3Stats.combatantStats.combatantName == "MissingNo.")
                enemy3Pick = -1;
            else
                enemy3Pick = 0;
        }
        //*/

        UpdateHealthDisplay(playerHealthPanel, player, true);
        UpdateHealthDisplay(minionHealthPanel, minion, false);
        UpdateHealthDisplay(enemy1HealthPanel, enemy1, true);
        UpdateHealthDisplay(enemy2HealthPanel, enemy2, enemy2Pick >= 0);
        UpdateHealthDisplay(enemy3HealthPanel, enemy3, enemy3Pick >= 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHealthDisplay(GameObject healthPanel, GameObject spriteObj, bool enable)
    {
        Stats stats = spriteObj.GetComponent<Stats>();
        TMP_Text text = healthPanel.transform.Find("HealthText").GetComponent<TMP_Text>();

        if (text != null && stats != null)
            text.text = "" + stats.health + "/" + stats.maxHealth;
        else
            text.text = "???";

        if (healthPanel.activeSelf != enable)
            healthPanel.SetActive(enable);

        if (spriteObj.activeSelf != enable)
            spriteObj.SetActive(enable);
        
        //Debug.Log(healthPanel.name + " " + healthPanel.activeSelf);
    }

    public void DoTurn()
    {
        //TODO: simulate whole turn based on player's desired action(s)
    }
}