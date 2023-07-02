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
        enemy1Stats.combatantStats = Resources.Load<Combatant>("Combatants/Rat");
        enemy1Stats.UpdateStats();

        //TODO: enable health displays for enemies and for minion (if summoned)
        UpdateHealthDisplay(playerHealthPanel, player, true);
        UpdateHealthDisplay(enemy1HealthPanel, enemy1, true);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSprite(GameObject obj, bool active)
    {
        //TODO: update sprite of the given game object with a given new sprite option
        obj.SetActive(active);
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
            healthPanel.SetActive(true);
        Debug.Log(healthPanel.name + " " + healthPanel.activeSelf);
    }

    public void DoTurn()
    {
        //TODO: simulate whole turn based on player's desired action(s)
    }
}