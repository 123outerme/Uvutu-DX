using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public enum SaveFormat : int
{
    PlayerInfo = 0,
    PStats = 1,
    Quests = 2,
    Inventory = 3,
    NPCDictionary = 4,
    Minion = 5,
    Enemy1 = 6,
    Enemy2 = 7,
    Enemy3 = 8,
    PlayerAction = 9,
    MinionAction = 10,
    Enemy1Action = 11,
    Enemy2Action = 12,
    Enemy3Action = 13,
    CurrentBattleState = 14,
    SaveFileLength = 15
}

public class SaveHandler : MonoBehaviour
{
    public string saveDirectory = "save";
    public string saveFile = "save1.txt";
    
    public GameObject playerDataObj;

    public bool loadDataOnInit = true;

    private PlayerInfo playerInfo;
    private Stats playerStats;

    private QuestInventory quests;
    private Inventory inventory;
    private NPCDict npcDict;

    private BattleHandler battleHandler = null;

    //NOTE: the reason why I keep local copies of the above and below is so that Loading/Saving battle state while in the pause menu and quitting keeps the state long enough to write
    private GameObject minion = null;
    private GameObject enemy1 = null;
    private GameObject enemy2 = null;
    private GameObject enemy3 = null;

    private Stats minionStats = null;
    private Stats enemy1Stats = null;
    private Stats enemy2Stats = null;
    private Stats enemy3Stats = null;

    private BattleAction playerAction = null;
    private BattleAction minionAction = null;
    private BattleAction enemy1Action = null;
    private BattleAction enemy2Action = null;
    private BattleAction enemy3Action = null;

    private BattleState currentBattleState = null;
    
    private string saveFilePath;

    // Start is called before the first frame update
    void Awake()
    {
        playerInfo = playerDataObj.GetComponent<PlayerInfo>();
        playerStats = playerDataObj.GetComponent<Stats>();
        quests = playerDataObj.GetComponent<QuestInventory>();
        inventory = playerDataObj.GetComponent<Inventory>();
        npcDict = new NPCDict();

        ComputeSavePath();
        if (loadDataOnInit)
            Load();
    }

    public void Save()
    {
        //for all NPCs in the current scene
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject npcObj in npcs)
        {
            NPCStats nStats = npcObj.GetComponent<NPCStats>();
            nStats.SaveNPCState();
            npcDict.Set(nStats.state);  //set state of that NPC
        }

        //save what last "gameplay" scene was (for pause menu, etc)
        Scene curScene = SceneManager.GetActiveScene();
        if (curScene.name == "Overworld")  //scene state that SHOULD be saved - world scene (overworld or underworld)
            playerInfo.scene = curScene.name;

        if ((curScene.name == "Battle" || playerInfo.inBattle) && !playerInfo.exitingBattle)  //if we're in battle/about to be in battle and not trying to exit back to the world
        {
            //save current battle state
            playerInfo.inBattle = true;
            
            TryGetBattleHandler();
        }
        else
            playerInfo.inBattle = false;

        playerInfo.exitingBattle = false;  //disable the exitingBattle flag no matter what, so the state doesn't mess up future saving

        //create save folder
        Directory.CreateDirectory(saveDirectory);

        string[] lines = CreateSaveFileText(playerInfo.inBattle);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            for(int i = 0; i < lines.Length; i++)
            {
                sw.WriteLine(lines[i]);
            }
        }
        playerInfo.loaded = true;
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            TryGetBattleHandler();

            string[] fileLines = LoadSaveFileText();
            
            bool usePosition = playerInfo.usePosition;  //keep usePosition property as the scene demands
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.PlayerInfo], playerInfo);                
            playerInfo.usePosition = usePosition;  //restore usePosition property after load
            
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.PStats], playerStats);
            playerStats.combatantStats = Resources.Load<Combatant>("Combatants/Player");  //load the player Combatant but do NOT overwrite the moveset or actual stats data (more for the sprite than anything)

            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Quests], quests);
            quests.LoadAllQuestDetails();  //load all Quests (for the details) into each quest tracker that was just loaded from save file
            
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Inventory], inventory);
            inventory.LoadAllInventorySlots();
            
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.NPCDictionary], npcDict);
            npcDict.SetAll();
            LoadNPCs();
            
            if (fileLines[(int) SaveFormat.Minion] != "" && minionStats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Minion], minionStats);
            
            if (fileLines[(int) SaveFormat.Enemy1] != "" && enemy1Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy1], enemy1Stats);

            if (fileLines[(int) SaveFormat.Enemy2] != "" && enemy2Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy2], enemy2Stats);

            if (fileLines[(int) SaveFormat.Enemy3] != "" && enemy3Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy3], enemy3Stats);

            if (fileLines[(int) SaveFormat.PlayerAction] != "" && playerAction != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.PlayerAction], playerAction);

            if (fileLines[(int) SaveFormat.MinionAction] != "" && minionAction != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.MinionAction], minionAction);

            if (fileLines[(int) SaveFormat.Enemy1Action] != "" && enemy1Action != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy1Action], enemy1Action);

            if (fileLines[(int) SaveFormat.Enemy2Action] != "" && enemy2Action != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy2Action], enemy2Action);

            if (fileLines[(int) SaveFormat.Enemy3Action] != "" && enemy3Action != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy3Action], enemy3Action);

            if (fileLines[(int) SaveFormat.CurrentBattleState] != "" && currentBattleState != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.CurrentBattleState], currentBattleState);
        }
    }

    public void LoadNPCs()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject npcObj in npcs)
        {
            NPCDialogue dialogue = npcObj.GetComponent<NPCDialogue>();
            NPCState state = npcDict.Get(npcObj.name);
            NPCStats nStats = npcObj.GetComponent<NPCStats>();
            nStats.LoadNPCState(state);
        }
    }

    public void AddPlayerSaveComponents()
    {
        playerInfo = playerDataObj.AddComponent<PlayerInfo>() as PlayerInfo;
        playerStats = playerDataObj.AddComponent<Stats>() as Stats;
        quests = playerDataObj.AddComponent<QuestInventory>() as QuestInventory;
        inventory = playerDataObj.AddComponent<Inventory>() as Inventory;
    }

    public void NewSave()
    {
        AddPlayerSaveComponents();
        playerStats.combatantStats = Resources.Load<Combatant>("Combatants/Player");
        playerStats.UpdateStats();
        playerStats.combatantName = "Uvutu";  //TODO give the player the ability to enter a custom name
        npcDict = new NPCDict();
        Save();
    }

    public bool SavesExist()
    {
        ComputeSavePath();
        return File.Exists(saveFilePath);
    }

    private void ComputeSavePath()
    {
        string[] paths = new string[] {saveDirectory, saveFile};

        saveFilePath = Path.Combine(paths);
    }

    private void TryGetBattleHandler()
    {
        GameObject handlerObj = GameObject.Find("BattleHandler");
        if (handlerObj != null)
            battleHandler = handlerObj.GetComponent<BattleHandler>();
        else
            battleHandler = null;

            minion = GameObject.Find("Minion");
            if (minion)
                minionStats = minion.GetComponent<Stats>();
            else
                minionStats = null;

            enemy1 = GameObject.Find("Enemy1");
            if (enemy1)
                enemy1Stats = enemy1.GetComponent<Stats>();
            else
                enemy1Stats = null;

            enemy2 = GameObject.Find("Enemy2");
            if (enemy2)
                enemy2Stats = enemy2.GetComponent<Stats>();
            else
                enemy2Stats = null;

            enemy3 = GameObject.Find("Enemy3");
            if (enemy3)
                enemy3Stats = enemy3.GetComponent<Stats>();
            else
                enemy3Stats = null;
            
            if (battleHandler != null)
            {
                playerAction = battleHandler.playerAction;
                minionAction = battleHandler.minionAction;
                enemy1Action = battleHandler.enemy1Action;
                enemy2Action = battleHandler.enemy2Action;
                enemy3Action = battleHandler.enemy3Action;
                currentBattleState = battleHandler.battleState;
            }
            else
            {
                playerAction = null;
                minionAction = null;
                enemy1Action = null;
                enemy2Action = null;
                enemy3Action = null;
                currentBattleState = null;
            }
    }

    public string GetSceneToLoad()
    {
        if (playerInfo.inBattle)
            return "Battle";
        else
            return playerInfo.scene;
    }

    private string[] CreateSaveFileText(bool keepOldBattleData)
    {
        string[] oldSaveText = LoadSaveFileText();

        string[] saveText = new string[(int) SaveFormat.SaveFileLength];
        saveText[(int) SaveFormat.PlayerInfo] = JsonUtility.ToJson(playerInfo);
        saveText[(int) SaveFormat.PStats] = JsonUtility.ToJson(playerStats);
        saveText[(int) SaveFormat.Quests] = JsonUtility.ToJson(quests);
        inventory.RemoveEmptyItemSlots();  //pretty up the state of the inventory
        saveText[(int) SaveFormat.Inventory] = JsonUtility.ToJson(inventory);
        
        npcDict.GetAll();
        saveText[(int) SaveFormat.NPCDictionary] = JsonUtility.ToJson(npcDict);


        string statsStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Minion] : "";
        if (battleHandler != null)
            statsStr = JsonUtility.ToJson(minionStats);
        saveText[(int) SaveFormat.Minion] = statsStr;

        statsStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy1] : "";
        if (battleHandler != null)
            statsStr = JsonUtility.ToJson(enemy1Stats);
        saveText[(int) SaveFormat.Enemy1] = statsStr;

        statsStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy2] : "";
        if (battleHandler != null)
            statsStr = JsonUtility.ToJson(enemy2Stats);
        saveText[(int) SaveFormat.Enemy2] = statsStr;

        statsStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy3] : "";
        if (battleHandler != null)
            statsStr = JsonUtility.ToJson(enemy3Stats);
        saveText[(int) SaveFormat.Enemy3] = statsStr;

        string actionStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.PlayerAction] : "";
        if (battleHandler != null)
            actionStr = JsonUtility.ToJson(playerAction);
        saveText[(int) SaveFormat.PlayerAction] = actionStr;

        actionStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.MinionAction] : "";
        if (battleHandler != null)
            actionStr = JsonUtility.ToJson(minionAction);
        saveText[(int) SaveFormat.MinionAction] = actionStr;

        actionStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy1Action] : "";
        if (battleHandler != null)
            actionStr = JsonUtility.ToJson(enemy1Action);
        saveText[(int) SaveFormat.Enemy1Action] = actionStr;

        actionStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy2Action] : "";
        if (battleHandler != null)
            actionStr = JsonUtility.ToJson(enemy2Action);
        saveText[(int) SaveFormat.Enemy2Action] = actionStr;

        actionStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.Enemy3Action] : "";
        if (battleHandler != null)
            actionStr = JsonUtility.ToJson(enemy3Action);
        saveText[(int) SaveFormat.Enemy3Action] = actionStr;

        string battleStateStr = (keepOldBattleData) ? oldSaveText[(int) SaveFormat.CurrentBattleState] : "";
        if (battleHandler != null)
            battleStateStr = JsonUtility.ToJson(currentBattleState);
        saveText[(int) SaveFormat.CurrentBattleState] = battleStateStr;
        
        return saveText;
    }

    private string[] LoadSaveFileText()
    {
        if (File.Exists(saveFilePath))
        {
            List<string> saveText = new List<string>();
            using (StreamReader sr = File.OpenText(saveFilePath))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                    saveText.Add(line);
            }
            return saveText.ToArray();
        }
        else return new string[(int) SaveFormat.SaveFileLength];
    }
}
