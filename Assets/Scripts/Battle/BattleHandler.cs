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

public enum BattleView
{
    SummonPrompt,
    Command,
    Attack,
    Target,
    UseItem,
    TurnActions
}

[System.Serializable]
public class BattleState
{
    public BattleView view;
    public bool commandingMinion;
    public int turnCount;
    public bool battleStarted = false;
    public string selectedTarget;

    public BattleState()
    {
        view = BattleView.SummonPrompt;
        commandingMinion = false;
        turnCount = 0;
        battleStarted = false;
        selectedTarget = "";
    }

    public void IncrementTurn()
    {
        turnCount++;
    }

    public void StartBattle()
    {
        battleStarted = true;
    }

    public void SetSelectedTarget(GameObject target)
    {
        selectedTarget = target.name;
    }
}

[System.Serializable]
public class BattleAction
{
    public BattleActionType type = BattleActionType.None;
    
    [System.NonSerialized]
    public Move move = null;

    public string moveName = "";

    [System.NonSerialized]
    public InventorySlot item = null;

    public string itemName = "";

    public List<string> targetStrings = new List<string>();
    private List<GameObject> targets = new List<GameObject>();
    
    public string nameUser = "";
    private GameObject user = null;

    public BattleAction(GameObject actionUser)
    {
        user = actionUser;
        nameUser = actionUser.name;
    }

    public void FetchMove()
    {
        if (moveName != "")
            move = Resources.Load<Move>("Moves/" + moveName);
    }

    public void LoadItemSlot(InventorySlot slot)
    {
        item = slot;
        itemName = slot.itemName;
    }

    public void FetchTargetsFromStrings()
    {
        targets = new List<GameObject>();
        foreach(string s in targetStrings)
        {
            targets.Add(GameObject.Find(s));
        }
    }

    public List<GameObject> GetTargetObjs()
    {
        return targets;
    }

    public void FetchUser()
    {
        if (nameUser != "")
            user = GameObject.Find(nameUser);
    }

    public GameObject GetUser()
    {
        if (user == null && nameUser != "")
            FetchUser();

        return user;
    }

    public void FetchAll()
    {
        FetchMove();
        FetchTargetsFromStrings();
        FetchUser();
    }

    public void Clear()
    {
        type = BattleActionType.None;
        move = null;
        moveName = "";
        item = null;
        itemName = "";
        targetStrings = new List<string>();
        targets = new List<GameObject>();
        nameUser = "";
        user = null;
    }
}

public class BattleActionPriorityComparer : PrioQueueComparer<BattleAction>
{
    public int Compare(BattleAction x, BattleAction y)
    {
        Stats sx = x.GetUser().GetComponent<Stats>();
        Stats sy = y.GetUser().GetComponent<Stats>();

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
    public GameObject overviewPanel;

    public BattleAction playerAction;
    public BattleAction minionAction;
    public BattleAction enemy1Action;
    public BattleAction enemy2Action;
    public BattleAction enemy3Action;

    public bool battleOverviewAvailable = true;

    //[System.NonSerialized]
    public BattleState battleState;

    private Stats playerStats;
    private Stats minionStats;
    private Stats enemy1Stats;
    private Stats enemy2Stats;
    private Stats enemy3Stats;

    private Inventory playerInventory;

    private PrioQueue<BattleAction, BattleAction> turnQueue = new PrioQueue<BattleAction, BattleAction>(new BattleActionPriorityComparer());
    public BattleAction currentTurn;
    private Dictionary<string, GameObject> nameToHealthPanelMap;
    private bool escaping = false;

    private string[] availableEnemyTypes; //TODO

    public BattleOverview overview;

    // Start is called before the first frame update
    void Start()
    {
        nameToHealthPanelMap = new Dictionary<string, GameObject>();
        nameToHealthPanelMap[player.name] = playerHealthPanel;
        nameToHealthPanelMap[minion.name] = minionHealthPanel;
        nameToHealthPanelMap[enemy1.name] = enemy1HealthPanel;
        nameToHealthPanelMap[enemy2.name] = enemy2HealthPanel;
        nameToHealthPanelMap[enemy3.name] = enemy3HealthPanel;

        bool playerActive = false, minionActive = false, enemy1Active = false, enemy2Active = false, enemy3Active = false;

        playerStats = player.GetComponent<Stats>();
        minionStats = minion.GetComponent<Stats>();
        enemy1Stats = enemy1.GetComponent<Stats>();
        //Debug.Log(enemy1Stats.combatantStats.combatantName);
        enemy2Stats = enemy2.GetComponent<Stats>();
        enemy3Stats = enemy3.GetComponent<Stats>();

        playerInventory = player.GetComponent<Inventory>();

        overview = overviewPanel.GetComponent<BattleOverview>();

        if (!battleState.battleStarted)
        {
            //new battle
            battleState = new BattleState();

             //TODO: decide upon selection of enemy types
            List<string> enemyOptions = new List<string> {"Combatants/Ant", "Combatants/Rat"};
            //randomly generate 1-3 enemies to fight
            //TODO: weighted randomness, random level within appropriate range for location
            int enemy1Pick = Mathf.RoundToInt(Random.Range(0, enemyOptions.Count));
            int enemy2Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
            int enemy3Pick = Mathf.RoundToInt(Random.Range(-1, enemyOptions.Count));
            //Debug.Log("" + enemyOptions.Count + " , " + enemy1Pick + " , " + enemy2Pick + " , " + enemy3Pick);

            playerActive = true;
            enemy1Active = true;

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
                enemy2Active = true;
            }

            if (enemy3Pick >= 0)
            {
                enemy3Stats.combatantStats = Resources.Load<Combatant>(enemyOptions[enemy3Pick]);
                enemy3Stats.UpdateStats();
                enemy3Active = true;
            }
        }
        else
        {
            //load battle data from pause or save during battle
            //set enabled flag for below
            if (!(playerStats.combatantStats.combatantName == "MissingNo." || playerStats.health <= 0))
                playerActive = true;

            if (!(minionStats.combatantStats.combatantName == "MissingNo." || minionStats.health <= 0))
                minionActive = true;

            if (!(enemy1Stats.combatantStats.combatantName == "MissingNo." || enemy1Stats.health <= 0))
                enemy1Active = true;

            if (!(enemy2Stats.combatantStats.combatantName == "MissingNo." || enemy2Stats.health <= 0))
                enemy2Active = true;

            if (!(enemy3Stats.combatantStats.combatantName == "MissingNo." || enemy3Stats.health <= 0))
                enemy3Active = true;

            if (playerAction != null)
            {
                playerAction.FetchAll();
                if (playerAction.itemName != "")
                {
                    InventorySlot slot = playerInventory.GetItemSlot(playerAction.itemName);
                    playerAction.LoadItemSlot(slot);
                }
                if (playerAction.targetStrings.Count > 0)
                    turnQueue.Enqueue(playerAction, playerAction);
            }
            
            if (minionAction != null)
            {
                minionAction.FetchAll();
                if (minionAction.itemName != "")
                {
                    InventorySlot slot = playerInventory.GetItemSlot(minionAction.itemName);
                    minionAction.LoadItemSlot(slot);
                }

                if (minionAction.targetStrings.Count > 0)
                    turnQueue.Enqueue(minionAction, minionAction);
            }
            
            if (enemy1Action != null)
            {
                enemy1Action.FetchAll();
                if (enemy1Action.targetStrings.Count > 0)
                    turnQueue.Enqueue(enemy1Action, enemy1Action);
            }

            if (enemy2Action != null)
            {
                enemy2Action.FetchAll();
                if (enemy2Action.targetStrings.Count > 0)
                    turnQueue.Enqueue(enemy2Action, enemy2Action);
            }
            if (enemy3Action != null)
            {
                enemy3Action.FetchAll();
                if (enemy3Action.targetStrings.Count > 0)
                    turnQueue.Enqueue(enemy3Action, enemy3Action);
            }
        }

        UpdateHealthDisplay(player, playerActive);
        UpdateHealthDisplay(minion, minionActive);
        UpdateHealthDisplay(enemy1, enemy1Active);
        UpdateHealthDisplay(enemy2, enemy2Active);
        UpdateHealthDisplay(enemy3, enemy3Active);
        battleState.StartBattle();
        UpdateView(battleState.view);  //setup & update battle view
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTurn()
    {
        //start a new turn, refreshing/updating all variables
        battleState.IncrementTurn();

        battleState.commandingMinion = !player.activeSelf;  //if player is downed, then the minion is the only one to be commanded, otherwise the minion waits until the player's done
        UpdateView(BattleView.Command);
        SetBattleOverviewShowButton(true);  //re-enable the battle overview button
        SetActionTarget("");
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
        
        LoadMinionSummon("Rat");  //TEMP: testing summon feature
    }

    public void LoadMinionSummon(string summonName)
    {
        if (summonName != "")
        {
            minionStats.combatantStats = Resources.Load<Combatant>("Combatants/" + summonName);
            minionStats.UpdateStats();

            UpdateHealthDisplay(minion, true);
        }

        overview.UpdateAllTabDetails();
        StartTurn();
    }

    public void ChooseAttack()
    {
        UpdateView(BattleView.Attack);
    }

    public void LoadAttacksUI()
    {
        Stats currentStats;
        if (!battleState.commandingMinion)
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
    }

    public void SelectAttack(int attackIndex)
    {
        //queue attack at player or minion's Stats.moveset[attackIndex]
        BattleAction action = minionAction;
        Stats stats = minionStats;

        if (!battleState.commandingMinion)
        {
            action = playerAction;
            stats = playerStats;
        }

        Debug.Log(playerAction);
        action.moveName = stats.moveset[attackIndex];
        action.FetchMove();
        action.type = BattleActionType.Move;

        if (action.move.validTargets != ValidBattleTarget.AllAllies && action.move.validTargets != ValidBattleTarget.AllEnemies)
        {
            UpdateView(BattleView.Target);  //SelectActionTarget() will be called from this
        }
        else
        {
            action.targetStrings = new List<string>();
            if (action.move.validTargets == ValidBattleTarget.AllAllies)
            {
                action.targetStrings.Add("Player");
                action.targetStrings.Add("Minion");
            }
            else
            {
                //action.move.validTargets == ValidBattleTarget.AllEnemies
                action.targetStrings.Add("Enemy1");
                action.targetStrings.Add("Enemy2");
                action.targetStrings.Add("Enemy3");
            }
            CompleteCommand();
        }
    }

    public void SelectActionTarget()
    {
        string targetFor = "";
        BattleAction action = minionAction;
        if (!battleState.commandingMinion)
            action = playerAction;
        else
            action = minionAction;
        
        targetFor += action.nameUser + "'s ";

        if (action.type == BattleActionType.Move)
            targetFor += action.moveName;
        else
            targetFor += action.itemName;

        TMP_Text targetForText = targetPanel.transform.Find("TargetForText").GetComponent<TMP_Text>();
        targetForText.text = "Select a target for " + targetFor + ":";

        //enable selection GUI for valid targets only
        List<string> validTargets = new List<string>();

        if (action.move.validTargets == ValidBattleTarget.Self)
            validTargets.Add(action.nameUser);

        if (action.move.validTargets == ValidBattleTarget.Ally || action.move.validTargets == ValidBattleTarget.Allies)
        {
            string allyName = "Minion";
            if (battleState.commandingMinion)
                allyName = "Player";

            validTargets.Add(allyName);
        }

        if (action.move.validTargets == ValidBattleTarget.Enemy)
        {
            validTargets.Add("Enemy1");
            validTargets.Add("Enemy2");
            validTargets.Add("Enemy3");
        }

        playerTargetSprite.SetActive(false);
        minionTargetSprite.SetActive(false);
        enemy1TargetSprite.SetActive(false);
        enemy2TargetSprite.SetActive(false);
        enemy3TargetSprite.SetActive(false);

        foreach(string target in validTargets)
        {
            if (target == "Player" && player.activeSelf)
                playerTargetSprite.SetActive(true);

            if (target == "Minion" && minion.activeSelf)
                minionTargetSprite.SetActive(true);

            if (target == "Enemy1" && enemy1.activeSelf)
                enemy1TargetSprite.SetActive(true);
            
            if (target == "Enemy2" && enemy2.activeSelf)
                enemy2TargetSprite.SetActive(true);

            if (target == "Enemy3" && enemy3.activeSelf)
                enemy3TargetSprite.SetActive(true);
        }
    }

    public void CancelSelectActionTarget()
    {
        UpdateView(BattleView.Attack);
    }

    public void SetActionTarget(string target)
    {
        GameObject buttonObj = targetPanel.transform.Find("ConfirmButton").gameObject;
        buttonObj.GetComponent<Button>().interactable = (target != "");  //true if target exists, false otherwise
        TMP_Text targetText = targetPanel.transform.Find("TargetNameText").GetComponent<TMP_Text>();
        battleState.selectedTarget = target;
        if (target != "")
        {
            Stats targetStats = GameObject.Find(target).GetComponent<Stats>();
            targetText.text = targetStats.combatantName + " (" + target + ")";
            if (!battleState.commandingMinion)
            {
                playerAction.targetStrings = new List<string>();
                playerAction.targetStrings.Add(target);
            }
            else
            {
                minionAction.targetStrings = new List<string>();
                minionAction.targetStrings.Add(target);
            }
        }
        else
            targetText.text = "None";  //reset text to selection = none
    }

    public void ConfirmTarget()
    {
        //any other necessary target processing?
        CompleteCommand();
    }

    public void ChooseItems()
    {
        //TODO
    }

    public void ChooseGuard()
    {
        //TODO
        if (!battleState.commandingMinion)
        {
            playerAction.move = Resources.Load<Move>("Moves/Guard");
            playerAction.targetStrings = new List<string>();
            playerAction.targetStrings.Add("Player");
            playerAction.type = BattleActionType.Move;
        }
        else
        {
            minionAction.move = Resources.Load<Move>("Moves/Guard");
            minionAction.targetStrings = new List<string>();
            minionAction.targetStrings.Add("Minion");
            minionAction.type = BattleActionType.Move;
        }

        CompleteCommand();
    }

    public void ChooseEscape()
    {
        if (!battleState.commandingMinion)
            playerAction.type = BattleActionType.Escape;
        else
            minionAction.type = BattleActionType.Escape;
        
        CompleteCommand();
    }

    public void ChooseBackToPlayerCommand()
    {
        battleState.commandingMinion = false;
        SetCommandMenuUI();
    }

    public void ReturnToCommand()
    {
        UpdateView(BattleView.Command);
    }

    public void CompleteCommand()
    {
        if (!battleState.commandingMinion)
        {
            //commanding player: if minion is present, command the minion and gray-out or hide some player-specific UI like items
            if (minion.activeSelf)
            {
                battleState.commandingMinion = true;
                battleState.selectedTarget = "";
                
                UpdateView(BattleView.Command);
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
            battleState.commandingMinion = false;
            DoTurn();
        }
    }

    private void SetCommandMenuUI()
    {
        //TODO: hide some player-specific UI like items or some minion-specific UI like "back to player command"
        TMP_Text chooseCommandNameText = commandPanel.transform.Find("ChooseCommandNameText").GetComponent<TMP_Text>();

        GameObject backButton = commandPanel.transform.Find("BackButton").gameObject;

        if (!battleState.commandingMinion)
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
            backButton.SetActive(player.activeSelf);  //set the back button active if the player can still be controlled, inactive otherwise
        }
    }

    public void DoTurn()
    {
        Debug.Log("do turn");

        //TODO: simulate whole turn based on player's desired action(s)
        //first: generate enemy actions and targets based on their "tendencies" and the battle status
        //and sort actions based on combatants' speed (and some random factors?)
        turnQueue = new PrioQueue<BattleAction, BattleAction>(new BattleActionPriorityComparer());

        if (player.activeSelf)
            turnQueue.Enqueue(playerAction, playerAction);
        
        if (minion.activeSelf)
            turnQueue.Enqueue(minionAction, minionAction);

        if (enemy1.activeSelf)
        {
            GetEnemyAIDecision(enemy1Stats, enemy1Action);
            turnQueue.Enqueue(enemy1Action, enemy1Action);
        }
        
        if (enemy2.activeSelf)
        {
            GetEnemyAIDecision(enemy2Stats, enemy2Action);
            turnQueue.Enqueue(enemy2Action, enemy2Action);
        }
        
        if (enemy3.activeSelf)
        {
            GetEnemyAIDecision(enemy3Stats, enemy3Action);
            turnQueue.Enqueue(enemy3Action, enemy3Action);
        }

        //finally: play out each action, updating statuses, etc.
        UpdateView(BattleView.TurnActions);
        DoNextAction();
    }

    public void DoNextAction()
    {
        //do next action, updating statuses, etc.
        //- if move, use the move on the intended target
        //- if item, use the item's effects on the intended target
        //- if flee, attempt to flee
        if (turnQueue.GetCount() > 0)
        {
            Stats userStats = null;
            currentTurn = null;
            while(userStats == null)  //go until we find an action where the user isn't downed
            {
                currentTurn = turnQueue.Peek();

                if (currentTurn == null)  //if the queue is empty
                {
                    StartTurn();  // the turn is now over
                    return;
                }

                userStats = currentTurn.GetUser().GetComponent<Stats>();  //get user's stats

                if (userStats.health <= 0)  //if the user is downed
                {
                    turnQueue.Dequeue();  //dequeue the turn, removing it from the list
                    userStats = null;  //set stats to null, signaling to the while loop to keep going until we find one where a user isn't downed, or we completed the turn
                }
            }
            currentTurn.FetchTargetsFromStrings();

            Debug.Log("action " + currentTurn.type);
            
            if (currentTurn.type == BattleActionType.Move)
            {
                for(int i = 0; i < currentTurn.GetTargetObjs().Count; i++)
                {
                    if (currentTurn.GetTargetObjs()[i] != null)
                    {
                        Stats targetStats = currentTurn.GetTargetObjs()[i].GetComponent<Stats>();
                        if (targetStats.health > 0)
                        {                
                            if (currentTurn.move.validTargets == ValidBattleTarget.Enemy || currentTurn.move.validTargets == ValidBattleTarget.AllEnemies)  //damage on an enemy
                            {
                                int damage = GetCurrentTurnDamageOnTarget(targetStats);
                                targetStats.health -= damage;
                                Debug.Log(damage);
                                UpdateHealthDisplay(currentTurn.GetTargetObjs()[i], true);
                            }
                            else
                            {
                                //valid targets == self, ally, allies, or all allies ; benefits to a self/ally/allies
                                //TODO affect self/ally/allies
                            }
                        }
                    }
                }
                userStats.RecieveMultipliers(currentTurn.move);
            }

            if (currentTurn.type == BattleActionType.UseItem)
            {
                //TODO: use item on target(s)
            }

            if (currentTurn.type == BattleActionType.Escape)
            {
                //TODO: calculate escape chances, generate a random number, compare for finding escape
                escaping = true;  //in the meantime: 100% escape chance
            }

            UpdateTurnActionText();
        }
        else
            StartTurn();  //start a new turn
    }

    public void UpdateTurnActionText()
    {
        Stats userStats = currentTurn.GetUser().GetComponent<Stats>();  //get user's stats
        string actionText = userStats.combatantName + " passed.";

        if (currentTurn.type == BattleActionType.Move)
        {   
            actionText = userStats.combatantName;
            if (currentTurn.move.validTargets == ValidBattleTarget.Enemy || currentTurn.move.validTargets == ValidBattleTarget.AllEnemies)
                actionText += " attacked with " + currentTurn.move.moveName;  //using a move on enemies
            else
                actionText += " used " + currentTurn.move.moveName;  //using a move on self, allies

            if (currentTurn.move.attackPower > 0)
                actionText +=  ", dealing";  //dealt damage
            else if (currentTurn.move.attackPower < 0)
                actionText += ", healing"; //healing damage
            else
                actionText += "";  //TODO figure out what to say with buffs/debuffs
            for(int i = 0; i < currentTurn.GetTargetObjs().Count; i++)
            {
                if (currentTurn.GetTargetObjs()[i] != null)
                {
                    Stats targetStats = currentTurn.GetTargetObjs()[i].GetComponent<Stats>();
                    if (targetStats.health > 0)
                    {                
                        if (currentTurn.move.validTargets == ValidBattleTarget.Enemy || currentTurn.move.validTargets == ValidBattleTarget.AllEnemies)  //damage on an enemy
                        {
                            //and update actionText for the full diplay of the turn details
                            int damage = GetCurrentTurnDamageOnTarget(targetStats);
                            if (damage > 0)  //damage, not healing
                                actionText += " " + damage + " damage to " + targetStats.combatantName;
                            else
                                actionText += " " + targetStats.combatantName + " by " + (-1 * damage) + "HP";
                        }
                        else
                        {
                            //valid targets == self, ally, allies, or all allies ; benefits to a self/ally/allies
                            //TODO
                            if (targetStats.combatantName != userStats.combatantName)  //if target is NOT self
                                actionText += ", buffing " + targetStats.combatantName;
                            else
                                actionText += " on self";
                        }
                    }
                    else
                    {
                        //previous valid target has no health now
                        if (currentTurn.move.validTargets == ValidBattleTarget.Enemy || currentTurn.move.validTargets == ValidBattleTarget.AllEnemies)  //
                        {
                            actionText += " overkill damage to " + targetStats.combatantName;
                        }
                        else
                        {
                            //valid targets == self, ally, allies, or all allies ; benefits to a self/ally/allies
                            actionText += ", but too little, too late for " + targetStats.combatantName;
                        }
                    }
                }
                else
                {
                    if (currentTurn.move.validTargets == ValidBattleTarget.Enemy || currentTurn.move.validTargets == ValidBattleTarget.AllEnemies)  //
                    {
                        actionText += " a wild blow to nothing";
                    }
                    else
                    {
                        //valid targets == self, ally, allies, or all allies ; benefits to a self/ally/allies
                        actionText += ", but no one could receive it";
                    }
                }

                if (i < currentTurn.GetTargetObjs().Count - 1)
                {
                    if (currentTurn.GetTargetObjs().Count > 2)
                        actionText += ",";  //only add commas when there are 3+ targets
                    actionText += " ";  //always add the space with more than one target

                    if (i == currentTurn.GetTargetObjs().Count - 2)
                        actionText += "and";  //if this is the second-to-last target, add an "and"
                }
            }

            actionText += ".";

            if (currentTurn.move.HasMultipliers())
            {
                actionText += " " + userStats.combatantName + " boosts ";
                StatMultiplierText[] multipliers = currentTurn.move.GetMultipliers();

                for(int i = 0; i < multipliers.Length; i++)
                {
                    actionText += multipliers[i].statName + " " + multipliers[i].multiplier + "x";
                    if (i < multipliers.Length - 1)
                    {
                        if (multipliers.Length > 2)
                            actionText += ",";  //only add commas when there are 3+ targets
                        actionText += " ";  //always add the space with more than one target

                        if (i == multipliers.Length - 2)
                            actionText += "and ";  //if this is the second-to-last target, add an "and"
                    }
                }
                actionText += ".";
            }
        }
        
        if (currentTurn.type == BattleActionType.UseItem)
        {
            //TODO
            actionText = userStats.combatantName + " used an item.";
        }

        if (currentTurn.type == BattleActionType.Escape)
        {
            //TODO
            actionText = userStats.combatantName + " escaped the battle successfully!";
        }

        UpdateTurnPanel(actionText);
    }

    public int GetCurrentTurnDamageOnTarget(Stats targetStats)
    {
        Stats userStats = currentTurn.GetUser().GetComponent<Stats>();
        float damage = currentTurn.move.attackPower;
        if (currentTurn.move.isMagic)
        {
            damage += userStats.magicAttack * userStats.magicAttackMultiplier;
        }
        else
        {
            damage += userStats.physAttack * userStats.physAttackMultiplier;
        }
        //TODO take into account target's resistance and resistance multiplier better
        damage -= targetStats.resistance * targetStats.resistanceMultiplier;

        return Mathf.RoundToInt(damage);  //finally, round up decimal points of damage
    }

    public void FinishNextAction()
    {
        turnQueue.Remove(currentTurn);
        currentTurn.Clear(); //clear action so saving here won't repeat the same action again

        if (escaping)
        {
            //if the last thing that happened was the escape succeeded, instead of doing the next action, actually escape the battle
            EscapeBattle();
            return;  //don't show the next turn starting in the UI
        }

        CheckCombatantsDefeated();

        if (!escaping)
            DoNextAction();
    }

    private void CheckCombatantsDefeated()
    {
        //handle combatants dying after losing all HP, and defeating either side
        int downedPlayers = 0;
        if (playerStats.health <= 0 || !player.activeSelf)
        {
            if (player.activeSelf)
                UpdateHealthDisplay(player, false);
            downedPlayers++;
        }
        
        if (minionStats.health <= 0 || !minion.activeSelf)
        {
            if (minion.activeSelf)
                UpdateHealthDisplay(minion, false);
            downedPlayers++;
        }

        if (downedPlayers == 2)
        {
            //TODO: after leaving the battle, make sure whatever scene loaded into resets the player's position and stats
            escaping = true;
            UpdateTurnPanel("You have been defeated.");
        }

        int downedEnemies = 0;
        if (enemy1Stats.health <= 0 || !enemy1.activeSelf)
        {
            if (enemy1.activeSelf)
                UpdateHealthDisplay(enemy1, false);
            downedEnemies++;
        }

        if (enemy2Stats.health <= 0 || !enemy2.activeSelf)
        {
            if (enemy2.activeSelf)
                UpdateHealthDisplay(enemy2, false);
            downedEnemies++;
        }

        if (enemy3Stats.health <= 0 || !enemy3.activeSelf)
        {
            if (enemy3.activeSelf)
                UpdateHealthDisplay(enemy3, false);
            downedEnemies++;
        }

        if (downedEnemies == 3)
        {
            //TODO: reward the player and tell them what the rewards are, do level ups if necessary, then leave the battle
            escaping = true;
            UpdateTurnPanel("You defeated all the enemies!");
        }

        overview.UpdateAllTabDetails();  //update in case any enemies were removed from battle
    }

    private void UpdateTurnPanel(string text)
    {
        TMP_Text turnText = turnPanel.transform.Find("TurnText").GetComponent<TMP_Text>();
        turnText.text = text;

        RectTransform rt = turnText.gameObject.GetComponent<RectTransform>();

        Vector2 preferredDims = turnText.GetPreferredValues(turnText.text, rt.sizeDelta.x, -1);
        
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
                player.GetComponent<PlayerInfo>().exitingBattle = true;
                playerStats.ResetMultipliers();
                loadScript.ResumeGame();
            }
        }
    }

    private void GetEnemyAIDecision(Stats enemyStats, BattleAction enemyAction)
    {
        //TEMP: testing AI, always uses a Move and randomly selects one from the list, then randomly selects a valid target
        enemyAction.type = BattleActionType.Move;
        int moveIndex = Mathf.RoundToInt(Random.Range(0, enemyStats.moveset.Length));
        enemyAction.moveName = enemyStats.moveset[moveIndex];
        enemyAction.FetchMove();
        string[] targets = EnemyGetValidTargets(enemyAction);
        
        if (enemyAction.move.validTargets == ValidBattleTarget.AllAllies || enemyAction.move.validTargets == ValidBattleTarget.AllEnemies)
        {
            //all-targeting moves
            enemyAction.targetStrings = new List<string>();
            enemyAction.targetStrings.AddRange(targets);
        }
        else
        {
            //choose a single target
            int targetIndex = Mathf.RoundToInt(Random.Range(0, targets.Length));
            enemyAction.targetStrings = new List<string>();
            enemyAction.targetStrings.Add(targets[targetIndex]);
        }
    }

    private string[] EnemyGetValidTargets(BattleAction action)
    {
        if (action.move.validTargets == ValidBattleTarget.Enemy || action.move.validTargets == ValidBattleTarget.AllEnemies)  //any enemy
        {
            List<string> targets = new List<string>();
            
            if (player.activeSelf)
                targets.Add("Player");
            if (minion.activeSelf)
                targets.Add("Minion");

            return targets.ToArray();  
        }
        else
        {
            if (action.move.validTargets == ValidBattleTarget.Self)  //only self
                return new string[1] {action.nameUser};

            if (action.move.validTargets == ValidBattleTarget.Ally)  //non-self allies
            {
                List<string> targets = new List<string>();

                if (enemy1.activeSelf)
                    targets.Add("Enemy1");
                if (enemy2.activeSelf)
                    targets.Add("Enemy2");
                if (enemy3.activeSelf)
                    targets.Add("Enemy3");

                targets.Remove(action.nameUser);  //remove self from the list
                return targets.ToArray();
            }

            if (action.move.validTargets == ValidBattleTarget.Allies || action.move.validTargets == ValidBattleTarget.AllAllies)  //all allies, including self
            {
                List<string> targets = new List<string>();

                if (enemy1.activeSelf)
                    targets.Add("Enemy1");
                if (enemy2.activeSelf)
                    targets.Add("Enemy2");
                if (enemy3.activeSelf)
                    targets.Add("Enemy3");

                return targets.ToArray();
            }

            return new string[0];  //default: no targets were calculated
        }
    }

    public void ToggleBattleOverviewPanel()
    {
        if (overviewPanel.activeSelf || battleOverviewAvailable)  //if overview panel is active already or it's not and we have the overview available
        {
            overviewPanel.SetActive(!overviewPanel.activeSelf);

            SetBattleOverviewShowButton(battleOverviewAvailable);
            battleOverviewAvailable = false;
        }
    }

    public void SetBattleOverviewShowButton(bool active)
    {
        Button showButton = GameObject.Find("ShowBattleOverviewPanel").transform.Find("ShowButton").GetComponent<Button>();
        showButton.interactable = active;  //set the button interactable state
        battleOverviewAvailable = active;  //set the state tracker properly
    }

    public void UpdateView(BattleView view)
    {
        battleState.view = view;

        summonPanel.SetActive((battleState.view == BattleView.SummonPrompt));

        if (battleState.view == BattleView.Command)
            SetCommandMenuUI();
        commandPanel.SetActive((battleState.view == BattleView.Command));

        if (battleState.view == BattleView.Attack)
            LoadAttacksUI();
        attackPanel.SetActive((battleState.view == BattleView.Attack));

        if (battleState.view == BattleView.Target)
        {
            SelectActionTarget();
            SetActionTarget(battleState.selectedTarget);
        }
        targetPanel.SetActive((battleState.view == BattleView.Target));

        if (battleState.view == BattleView.TurnActions)
        {
            currentTurn = turnQueue.Peek();
            UpdateTurnActionText();
        }

        turnPanel.SetActive((battleState.view == BattleView.TurnActions));
    }
}