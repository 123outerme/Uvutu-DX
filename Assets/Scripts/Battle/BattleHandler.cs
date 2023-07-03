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
        //TODO: decide upon number of enemies and their types and stats
        Stats enemy1Stats = enemy1.GetComponent<Stats>();
        
        List<string> enemyOptions = new List<string> {"Combatants/Ant", "Combatants/Rat"};
        
        int enemy1Pick = Mathf.RoundToInt(Random.Range(0, enemyOptions.Count));
        
        enemy1Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy1Pick]);
        enemy1Stats.UpdateStats();

        int enemy2Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
        if (enemy2Pick >= 0)
        {
            Stats enemy2Stats = enemy2.GetComponent<Stats>();
            enemy2Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy2Pick]);
            enemy2Stats.UpdateStats();
        }

        int enemy3Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
        //Debug.Log("" + enemyOptions.Count + " , " + enemy1Pick + " , " + enemy2Pick + " , " + enemy3Pick);
        if (enemy2Pick >= 0 && enemy3Pick >= 0)
        {
            Stats enemy3Stats = enemy3.GetComponent<Stats>();
            enemy3Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy3Pick]);
            enemy3Stats.UpdateStats();
        }
        else
            enemy3Pick = -1;

        //TODO: enable health displays for enemies and for minion (if summoned)
        UpdateHealthDisplay(playerHealthPanel, player, true);
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