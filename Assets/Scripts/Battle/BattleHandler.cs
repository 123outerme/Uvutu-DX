using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum BattleActionType
{
    Move,
    UseItem,
    Escape,
    None
}

public class BattleAction
{
    public BattleActionType type = BattleActionType.None;
    public Move move = null;
    public List<GameObject> targets = new List<GameObject>();
    public InventoryItem item = null;
    public GameObject user = null;

    public BattleAction(GameObject actionUser)
    {
        user = actionUser;
    }
}

public class BattleActionPriorityComparer : PrioQueueComparer<BattleAction>
{
    public int Compare(BattleAction x, BattleAction y)
    {
        Stats sx = x.user.GetComponent<Stats>();
        Stats sy = y.user.GetComponent<Stats>();

        if (sx.speed == sy.speed)
            return 0;

        if (sx.speed > sy.speed)
            return 1;
        else
            return -1;
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
    public GameObject turnPanel;

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

    private PrioQueue<BattleAction, BattleAction> turnQueue;
    private Dictionary<string, GameObject> nameToHealthPanelMap;
    private bool escaping = false;

    private string[] availableEnemyTypes; //TODO

    // Start is called before the first frame update
    void Start()
    {
        nameToHealthPanelMap = new Dictionary<string, GameObject>();
        nameToHealthPanelMap[player.name] = playerHealthPanel;
        nameToHealthPanelMap[minion.name] = minionHealthPanel;
        nameToHealthPanelMap[enemy1.name] = enemy1HealthPanel;
        nameToHealthPanelMap[enemy2.name] = enemy2HealthPanel;
        nameToHealthPanelMap[enemy3.name] = enemy3HealthPanel;

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

        UpdateHealthDisplay(player, true);
        UpdateHealthDisplay(minion, false);
        UpdateHealthDisplay(enemy1, true);
        UpdateHealthDisplay(enemy2, enemy2Pick >= 0);
        UpdateHealthDisplay(enemy3, enemy3Pick >= 0);
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
        SetActionTarget(null);
        playerAction = new BattleAction(player);
        minionAction = new BattleAction(minion);
        enemy1Action = new BattleAction(enemy1);
        enemy2Action = new BattleAction(enemy2);
        enemy3Action = new BattleAction(enemy3);
    }

    void UpdateHealthDisplay(GameObject spriteObj, bool enable)
    {
        GameObject healthPanel = nameToHealthPanelMap[spriteObj.name];
        Stats stats = spriteObj.GetComponent<Stats>();
        TMP_Text text = healthPanel.transform.Find("HealthText").GetComponent<TMP_Text>();

        if (text != null && stats != null)
        {
            if (stats.health < 0)
                stats.health = 0;

            text.text = "L" + stats.level + ": " + stats.health + "/" + stats.maxHealth;
        }
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

            UpdateHealthDisplay(minion, true);
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
        action.type = BattleActionType.Move;

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
        GameObject buttonObj = targetPanel.transform.Find("ConfirmButton").gameObject;
        buttonObj.GetComponent<Button>().interactable = (target != null);  //true if target exists, false otherwise
        TMP_Text targetText = targetPanel.transform.Find("TargetNameText").GetComponent<TMP_Text>();
        if (target != null)
        {
            Stats targetStats = target.GetComponent<Stats>();
            targetText.text = targetStats.combatantName + " (" + target.name + ")";
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
        else
            targetText.text = "None";  //reset text to selection = none
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
            playerAction.type = BattleActionType.Escape;
        else
            minionAction.type = BattleActionType.Escape;
        
        CompleteCommand();
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
        turnQueue = new PrioQueue<BattleAction, BattleAction>(new BattleActionPriorityComparer());

        if (player.activeSelf)
            turnQueue.Enqueue(playerAction, playerAction);
        
        if (minion.activeSelf)
            turnQueue.Enqueue(minionAction, minionAction);

        if (enemy1.activeSelf)
            turnQueue.Enqueue(enemy1Action, enemy1Action);
        
        if (enemy2.activeSelf)
            turnQueue.Enqueue(enemy2Action, enemy2Action);
        
        if (enemy3.activeSelf)
            turnQueue.Enqueue(enemy3Action, enemy3Action);

        //finally: play out each action, updating statuses, etc.
        turnPanel.SetActive(true);
        DoNextAction();
    }

    public void DoNextAction()
    {
        if (escaping)  //if the last thing that happened was the escape succeeded, instead of doing the next action, actually escape the battle
            EscapeBattle();

        //TODO: handle combatants dying after losing all HP, and defeating either side

        //do next action, updating statuses, etc.
        //- if move, use the move on the intended target
        //- if item, use the item's effects on the intended target
        //- if flee, attempt to flee
        if (turnQueue.GetCount() > 0)
        {
            BattleAction action = turnQueue.Dequeue();
            Debug.Log("action " + action.type);

            Stats userStats = action.user.GetComponent<Stats>();

            string actionText = userStats.combatantName + " passed.";
            
            if (action.type == BattleActionType.Move)
            {
                //TODO: use move on target(s)

                actionText = userStats.combatantName;
                if (action.move.validTargets == ValidBattleTarget.Enemy || action.move.validTargets == ValidBattleTarget.AllEnemies)
                    actionText += " attacked with " + action.move.moveName;  //using a move on enemies
                else
                    actionText += " used ";  //using a move on self, allies

                if (action.move.attackPower > 0)
                    actionText +=  ", dealing ";  //dealt damage
                else
                    actionText += ""; //TODO figure out what to say with buffs/debuffs

                for(int i = 0; i < action.targets.Count; i++)
                {
                    Stats targetStats = action.targets[i].GetComponent<Stats>();                    

                    if (action.move.validTargets == ValidBattleTarget.Enemy || action.move.validTargets == ValidBattleTarget.AllEnemies)
                    {
                        float damage = action.move.attackPower;
                        if (action.move.isMagic)
                            damage *= userStats.magicAttack * userStats.magicAttackMultiplier;
                        else
                            damage *= userStats.physAttack * userStats.physAttackMultiplier;
                        //TODO take into account target's resistance and resistance multiplier

                        damage = Mathf.Round(damage);  //finally, round up decimal points of damage
                        targetStats.health -= (int) damage;
                        Debug.Log(damage);
                        UpdateHealthDisplay(action.targets[i], true);

                        userStats.RecieveMultipliers(action.move);

                        //and update actionText for the full diplay of the turn details
                        actionText += "" + ((int) damage) + " damage to " + targetStats.combatantName;
                        if (i < action.targets.Count - 1)
                        {
                            if (action.targets.Count > 2)
                                actionText += ",";  //only add commas when there are 3+ targets
                            actionText += " ";  //always add the space with more than one target

                            if (i == action.targets.Count - 2)
                                actionText += "and ";  //if this is the second-to-last target, add an "and"
                        }
                        else
                            actionText += ".";
                    }
                }
            }

            if (action.type == BattleActionType.UseItem)
            {
                //TODO: use item on target(s)
                actionText = userStats.combatantName + " used an item.";
            }

            if (action.type == BattleActionType.Escape)
            {
                //TODO: calculate escape chances, generate a random number, compare for finding escape
                escaping = true;  //in the meantime: 100% escape chance

                actionText = userStats.combatantName + " escaped the battle successfully!";
            }

            UpdateTurnPanel(actionText);
        }
        else
        {
            //after completion, start a new turn
            turnPanel.SetActive(false);
            commandPanel.SetActive(true);
            StartTurn();
        }
    }

    private void UpdateTurnPanel(string text)
    {
        Transform turnTextPanel = turnPanel.transform.Find("TurnTextPanel");
        TMP_Text turnText = turnTextPanel.Find("TurnText").GetComponent<TMP_Text>();
        turnText.text = text;

        Vector2 preferredDims = turnText.GetPreferredValues(text);

        RectTransform rt = turnTextPanel.gameObject.GetComponent<RectTransform>();
        //rt.sizeDelta = new Vector2(rt.sizeDelta.x, preferredDims.y);  //change the y dimensions to fit the string
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredDims.y);  //change the height to fit the string
        //NOTE: we have to use this function because we're on stretch mode and it is a different calc than setting the deltas between each corner of the RectTransform
    }

    private void EscapeBattle()
    {
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
}