using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum BattleActionType
{
    Move,
    UseItem,
    Flee,
    None
}

public class BattleAction
{
    public BattleActionType type = BattleActionType.None;
    public Move move = null;
    public List<GameObject> targets = new List<GameObject>();
    public InventoryItem item = null;
    public bool escape = false;
    public GameObject user = null;

    public BattleAction(GameObject actionUser)
    {
        user = actionUser;
    }
}

public class BattleHandler : MonoBehaviour
{
    public int turnCount = 0;

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

    public GameObject playerTargetSprite;
    public GameObject minionTargetSprite;
    public GameObject enemy1TargetSprite;
    public GameObject enemy2TargetSprite;
    public GameObject enemy3TargetSprite;

    public GameObject summonPanel;
    public GameObject commandPanel;
    public GameObject attackPanel;
    public GameObject targetPanel;

    private bool commandingMinion = false;

    private Stats playerStats;
    private Stats minionStats;
    private Stats enemy1Stats;
    private Stats enemy2Stats;
    private Stats enemy3Stats;

    private BattleAction playerAction;
    private BattleAction minionAction;
    private BattleAction enemy1Action;
    private BattleAction enemy2Action;
    private BattleAction enemy3Action;

    private string[] availableEnemyTypes; //TODO

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

    public void StartTurn()
    {
        //start a new turn, refreshing/updating all variables
        turnCount++;

        commandingMinion = false;
        SetCommandMenuUI();
        playerAction = new BattleAction(player);
        minionAction = new BattleAction(minion);
        enemy1Action = new BattleAction(enemy1);
        enemy2Action = new BattleAction(enemy2);
        enemy3Action = new BattleAction(enemy3);
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

        StartTurn();
    }

    public void ChooseAttack()
    {
        Stats currentStats;
        if (!commandingMinion)
            currentStats = player.GetComponent<Stats>();
        else
            currentStats = minion.GetComponent<Stats>();

        for(int i = 0; i < currentStats.moveset.Length; i++)
        {
            string buttonName = "Attack" + (i+1) + "Button";
            GameObject buttonObj = attackPanel.transform.Find(buttonName).gameObject;
            buttonObj.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = currentStats.moveset[i];
            buttonObj.GetComponent<Button>().interactable = true;
        }

        commandPanel.SetActive(false);
        attackPanel.SetActive(true);
    }

    public void SelectAttack(int attackIndex)
    {
        //queue attack at player or minion's Stats.moveset[attackIndex]
        string targetFor = "Minion";
        string moveName = "Attack";
        BattleAction action;
        string selfName = "";
        string allyName = "";

        if (!commandingMinion)
        {
            Debug.Log(playerStats.moveset[attackIndex]);
            playerAction.move = Resources.Load<Move>("Moves/" + playerStats.moveset[attackIndex]);
            moveName = playerAction.move.moveName;
            targetFor = playerStats.combatantName;
            action = playerAction;
            selfName = "Player";
            allyName = "Minion";
        }
        else
        {
            Debug.Log(minionStats.moveset[attackIndex]);
            minionAction.move = Resources.Load<Move>("Moves/" + minionStats.moveset[attackIndex]);
            moveName = minionAction.move.moveName;
            //targetFor already == "Minion"
            action = minionAction;
            selfName = "Minion";
            allyName = "Player";
        }

        attackPanel.SetActive(false);

        targetFor += "'s " + moveName;

        List<string> curValidTargets = new List<string>();

        if (action.move.validTargets == ValidBattleTarget.Self)
            curValidTargets.Add(selfName);

        if (action.move.validTargets == ValidBattleTarget.Ally || action.move.validTargets == ValidBattleTarget.Allies)
            curValidTargets.Add(allyName);

        if (action.move.validTargets == ValidBattleTarget.Enemy)
        {
            curValidTargets.Add("Enemy1");
            curValidTargets.Add("Enemy2");
            curValidTargets.Add("Enemy3");
        }

        if (action.move.validTargets != ValidBattleTarget.AllAllies && action.move.validTargets != ValidBattleTarget.AllEnemies)
        {
            SelectActionTarget(targetFor, curValidTargets.ToArray());
        }
        else
        {
            action.targets = new List<GameObject>();
            if (action.move.validTargets == ValidBattleTarget.AllAllies)
            {
                action.targets.Add(player);
                action.targets.Add(minion);
            }
            else
            {
                //action.move.validTargets == ValidBattleTarget.AllEnemies
                action.targets.Add(enemy1);
                action.targets.Add(enemy2);
                action.targets.Add(enemy3);
            }
            CompleteCommand();
        }
    }

    public void SelectActionTarget(string targetFor, string[] validTargets)
    {
        //NOTE: any panel active before this should be deactivated, and validTargets should be filled, before calling SelectActionTarget() since multiple types of actions & different target options
        
        TMP_Text targetForText = targetPanel.transform.Find("TargetForText").GetComponent<TMP_Text>();
        targetForText.text = "Select a target for " + targetFor + ":";

        //enable selection GUI for valid targets only
        playerTargetSprite.SetActive(false);
        minionTargetSprite.SetActive(false);
        enemy1TargetSprite.SetActive(false);
        enemy2TargetSprite.SetActive(false);
        enemy3TargetSprite.SetActive(false);

        foreach(string target in validTargets)
        {
            if (target == "Player")
                playerTargetSprite.SetActive(true);

            if (target == "Minion" && minion.activeSelf)
                minionTargetSprite.SetActive(true);

            if (target == "Enemy1")
                enemy1TargetSprite.SetActive(true);
            
            if (target == "Enemy2" && enemy2.activeSelf)
                enemy2TargetSprite.SetActive(true);

            if (target == "Enemy3" && enemy3.activeSelf)
                enemy3TargetSprite.SetActive(true);
        }

        targetPanel.SetActive(true);
    }

    public void CancelSelectActionTarget()
    {
        targetPanel.SetActive(false);
        attackPanel.SetActive(true);
    }

    public void SetActionTarget(GameObject target)
    {
        Stats targetStats = target.GetComponent<Stats>();
        TMP_Text targetText = targetPanel.transform.Find("TargetNameText").GetComponent<TMP_Text>();
        targetText.text = targetStats.combatantName + " (" + target.name + ")";

        GameObject buttonObj = targetPanel.transform.Find("ConfirmButton").gameObject;
        buttonObj.GetComponent<Button>().interactable = true;

        if (!commandingMinion)
        {
            playerAction.targets = new List<GameObject>();
            playerAction.targets.Add(target);
        }
        else
        {
            minionAction.targets = new List<GameObject>();
            minionAction.targets.Add(target);
        }
    }

    public void ConfirmTarget()
    {
        targetPanel.SetActive(false);
        CompleteCommand();
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
            playerAction.move = Resources.Load<Move>("Moves/Guard");
            playerAction.targets = new List<GameObject>();
            playerAction.targets.Add(player);
        }
        else
        {
            minionAction.move = Resources.Load<Move>("Moves/Guard");
            minionAction.targets = new List<GameObject>();
            minionAction.targets.Add(minion);
        }
        CompleteCommand();
    }

    public void ChooseEscape()
    {
        if (!commandingMinion)
            playerAction.escape = true;
        else
            minionAction.escape = true;
        
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

    public void ReturnToCommand(GameObject currentPanel)
    {
        currentPanel.SetActive(false);
        commandPanel.SetActive(true);
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

        GameObject backButton = commandPanel.transform.Find("BackButton").gameObject;

        if (!commandingMinion)
        {
            string playerName = "yourself";
            if (playerStats.combatantName != "")
                playerName = playerStats.combatantName;

            chooseCommandNameText.text = "Give a command for " + playerName + " to carry out:";
            backButton.SetActive(false);
        }
        else
        {
            chooseCommandNameText.text = "Give a command to your minion:";
            backButton.SetActive(true);
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

        //after completion, start a new turn
        commandPanel.SetActive(true);
        StartTurn();
    }
}