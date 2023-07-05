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

    public GameObject summonPanel;
    public GameObject commandPanel;
    public GameObject attackPanel;

    private bool commandingMinion = false;

    private Stats playerStats;
    private Stats minionStats;
    private Stats enemy1Stats;
    private Stats enemy2Stats;
    private Stats enemy3Stats;

    private Move playerMove = null;
    private GameObject playerMoveTarget = null;

    private Move minionMove = null;
    private GameObject minionMoveTarget = null;
    
    private InventoryItem useItem = null;
    private GameObject InventoryItemTarget = null;

    private bool playerEscape = false;
    private bool minionEscape = false;

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
        playerStats = player.GetComponent<Stats>();
        minionStats = minion.GetComponent<Stats>();
        enemy1Stats = enemy1.GetComponent<Stats>();
        //Debug.Log(enemy1Stats.combatantStats.combatantName);
        enemy2Stats = enemy2.GetComponent<Stats>();
        enemy3Stats = enemy3.GetComponent<Stats>();

        if (enemy1Stats.combatantStats.combatantName != "MissingNo.")
            startFromLoad = true;

        //*
        if (!startFromLoad)
        {
            //new battle
            playerStats.ResetMultipliers();
            minionStats.ResetMultipliers();
            enemy1Stats.ResetMultipliers();
            enemy2Stats.ResetMultipliers();
            enemy3Stats.ResetMultipliers();

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
            //load battle data from pause or save during battle
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
        SetCommandMenuUI();  //set command menu text for the first time
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
            text.text = "L" + stats.level + ": " + stats.health + "/" + stats.maxHealth;
        else
            text.text = "L???: ????/????";

        Vector2 preferredDims = text.GetPreferredValues(text.text);

        RectTransform rt = healthPanel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(preferredDims.x + 10, rt.sizeDelta.y);  //change the x dimensions to fit the string (with a bit of space on the left/right)

        if (healthPanel.activeSelf != enable)
            healthPanel.SetActive(enable);

        if (spriteObj.activeSelf != enable)
            spriteObj.SetActive(enable);
        
        //Debug.Log(healthPanel.name + " " + healthPanel.activeSelf);
    }

    public void ChooseMinionSummon()
    {
        //TODO open choose summon menu, then once picked activate the command panel for player turn
    }

    public void LoadMinionSummon(string summonName)
    {
        if (summonName != "")
        {
            minionStats.combatantStats = Resources.Load<Combatant>(summonName);
            minionStats.UpdateStats();

            UpdateHealthDisplay(minionHealthPanel, minion, true);
        }

        summonPanel.SetActive(false);
        commandPanel.SetActive(true);
    }

    public void ChooseAttack()
    {
        commandPanel.SetActive(false);
        attackPanel.SetActive(true);
    }

    public void SelectAttack(int attackIndex)
    {
        //queue attack at player or minion's Stats.moveset[attackIndex]
        if (!commandingMinion)
        {
            playerMove = playerStats.moveset[attackIndex];
        }
        else
        {
            Stats s = minion.GetComponent<Stats>();
            minionMove = playerStats.moveset[attackIndex];
        }

        SelectActionTarget();
    }

    public void SelectActionTarget()
    {
        //depending on action, change "select target" text
        //enable selection GUI
    }
       

    public void SetActionTarget()
    {
        if (!commandingMinion)
        {
            //set playerTarget
        }
        else
        {
            //set minionTarget
        }
    }

    public void ChooseItems()
    {
        //TODO
    }

    public void ChooseGuard()
    {
        //TODO
        if (!commandingMinion)
        {
            playerMove = Resources.Load<Move>("Guard");
            playerMoveTarget = player;
        }
        else
        {
            minionMove = Resources.Load<Move>("Guard");
            minionMoveTarget = minion;
        }
        CompleteCommand();
    }

    public void ChooseEscape()
    {
        if (!commandingMinion)
            playerEscape = true;
        else
            minionEscape = true;
        
        CompleteCommand();
        //TODO move the escape logic until after fleeing is going to work and notifying the player
        GameObject loader = GameObject.Find("SceneLoader");
        if (loader != null)
        {
            SceneLoader loadScript = loader.GetComponent<SceneLoader>();
            if (loadScript != null)
            {
                player.GetComponent<PlayerLocation>().exitingBattle = true;
                loadScript.ResumeGame();
            }
        }
    }

    public void ChooseBackToPlayerCommand()
    {
        commandingMinion = false;
        SetCommandMenuUI();
    }

    public void CompleteCommand()
    {
        if (!commandingMinion)
        {
            //commanding player: if minion is present, command the minion and gray-out or hide some player-specific UI like items
            if (minion.activeSelf)
            {
                commandingMinion = true;
                
                SetCommandMenuUI();
                commandPanel.SetActive(true);
            }
            else
            {
                //if minion is not present, then battle is ready to proceed
                DoTurn();
            }
        }
        else
        {
            //minion is done getting an action, so battle is certainly ready to proceed
            commandingMinion = false;
            SetCommandMenuUI();
            DoTurn();
        }
    }

    private void SetCommandMenuUI()
    {
        //TODO: hide some player-specific UI like items or some minion-specific UI like "back to player command"
        TMP_Text chooseCommandNameText = commandPanel.transform.Find("ChooseCommandNameText").GetComponent<TMP_Text>();
        if (!commandingMinion)
        {
            string playerName = "yourself";
            if (playerStats.combatantName != "")
                playerName = playerStats.combatantName;

            chooseCommandNameText.text = "Give a command for " + playerName + " to carry out:";
        }
        else
        {
            chooseCommandNameText.text = "Give a command to your minion:";
        }
    }

    public void DoTurn()
    {
        Debug.Log("do turn");
        //TODO: simulate whole turn based on player's desired action(s)

        //first: generate enemy actions and targets based on their "tendencies" and the battle status
        //then: sort actions based on combatants' speed (and some random factors?)
        //finally: play out each action, updating statuses, etc.
        //- if move, use the move on the intended target
        //- if item, use the item's effects on the intended target
        //- if flee, attempt to flee
    }
}