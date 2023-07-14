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
    Enemy1 = 5,
    Enemy2 = 6,
    Enemy3 = 7,
    SaveFileLength = 8
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
    
    private GameObject enemy1 = null;
    private GameObject enemy2 = null;
    private GameObject enemy3 = null;

    private Stats enemy1Stats = null;
    private Stats enemy2Stats = null;
    private Stats enemy3Stats = null;
    
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
            
            TryGetCombatantsStats();
        }
        else
            playerInfo.inBattle = false;

        playerInfo.exitingBattle = false;  //disable the exitingBattle flag no matter what, so the state doesn't mess up future saving

        //create save folder
        Directory.CreateDirectory(saveDirectory);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            string[] lines = CreateSaveFileText();

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
            TryGetCombatantsStats();

            string[] fileLines = LoadSaveFileText();
            bool usePosition = playerInfo.usePosition;  //keep usePosition property as the scene demands
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.PlayerInfo], playerInfo);                
            playerInfo.usePosition = usePosition;  //restore usePosition property after load
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.PStats], playerStats);
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Quests], quests);
            quests.LoadAllQuestDetails();  //load all Quests (for the details) into each quest tracker that was just loaded from save file
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Inventory], inventory);
            JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.NPCDictionary], npcDict);
            npcDict.SetAll();
            LoadNPCs();

            //*
            if (fileLines[(int) SaveFormat.Enemy1] != "" && enemy1Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy1], enemy1Stats);
            
            if (fileLines[(int) SaveFormat.Enemy2] != "" && enemy2Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy2], enemy2Stats);

            if (fileLines[(int) SaveFormat.Enemy3] != "" && enemy3Stats != null)
                JsonUtility.FromJsonOverwrite(fileLines[(int) SaveFormat.Enemy3], enemy3Stats);
            //*/
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

    private void TryGetCombatantsStats()
    {
        enemy1 = GameObject.Find("Enemy1");
        if (enemy1)  //enemy1 should always be active but just in case      
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
    }

    public string GetSceneToLoad()
    {
        if (playerInfo.inBattle)
            return "Battle";
        else
            return playerInfo.scene;
    }

    private string[] CreateSaveFileText()
    {
        string[] saveText = new string[(int) SaveFormat.SaveFileLength];
        saveText[(int) SaveFormat.PlayerInfo] = JsonUtility.ToJson(playerInfo);
        saveText[(int) SaveFormat.PStats] = JsonUtility.ToJson(playerStats);
        saveText[(int) SaveFormat.Quests] = JsonUtility.ToJson(quests);
        saveText[(int) SaveFormat.Inventory] = JsonUtility.ToJson(inventory);
        npcDict.GetAll();
        saveText[(int) SaveFormat.NPCDictionary] = JsonUtility.ToJson(npcDict);

        string enemy1Str = "";
        if (enemy1Stats != null)
            enemy1Str = JsonUtility.ToJson(enemy1Stats);
        saveText[(int) SaveFormat.Enemy1] = enemy1Str;

        string enemy2Str = "";
        if (enemy2Stats != null)
            enemy2Str = JsonUtility.ToJson(enemy2Stats);
        saveText[(int) SaveFormat.Enemy2] = enemy2Str;

        string enemy3Str = "";
        if (enemy3Stats != null)
            enemy3Str = JsonUtility.ToJson(enemy3Stats);
        saveText[(int) SaveFormat.Enemy3] = enemy3Str;

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
