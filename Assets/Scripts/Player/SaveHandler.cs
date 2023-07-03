using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    public string saveDirectory = "save";
    public string saveFile = "save1.txt";
    
    public GameObject playerDataObj;

    public bool loadDataOnInit = true;

    private PlayerLocation location;
    private Stats playerStats;

    private Quests quests;
    //private Inventory inventory;
    private NPCDict npcDict;
    
    private GameObject minion = null;
    private GameObject enemy1 = null;
    private GameObject enemy2 = null;
    private GameObject enemy3 = null;

    private Stats minionStats = null;
    private Stats enemy1Stats = null;
    private Stats enemy2Stats = null;
    private Stats enemy3Stats = null;
    
    private string saveFilePath;

    // Start is called before the first frame update
    void Awake()
    {
        location = playerDataObj.GetComponent<PlayerLocation>();
        playerStats = playerDataObj.GetComponent<Stats>();
        quests = playerDataObj.GetComponent<Quests>();
        //inventory = playerDataObj.GetComponent<Inventory>();
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
        if (curScene.name == "Overworld" || curScene.name == "Battle")  //scene state that SHOULD be saved
            location.scene = curScene.name;

        if (curScene.name == "Battle")
        {
            //save current battle state
            location.inBattle = true;
            
            TryGetCombatantsStats();
        }
        else
            location.inBattle = false;

        //create save folder
        Directory.CreateDirectory(saveDirectory);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            sw.WriteLine(JsonUtility.ToJson(location));
            sw.WriteLine(JsonUtility.ToJson(playerStats));
            sw.WriteLine(JsonUtility.ToJson(quests));
            //sw.WriteLine(JsonUtility.ToJson(inventory));
            npcDict.GetAll();
            sw.WriteLine(JsonUtility.ToJson(npcDict));

            if (minionStats != null)
                sw.WriteLine(JsonUtility.ToJson(minionStats));
            else
                sw.WriteLine("");

            if (enemy1Stats != null)
                sw.WriteLine(JsonUtility.ToJson(enemy1Stats));
            else
                sw.WriteLine("");

            if (enemy2Stats != null)
                sw.WriteLine(JsonUtility.ToJson(enemy2Stats));
            else
                sw.WriteLine("");

            if (enemy3Stats != null)
                sw.WriteLine(JsonUtility.ToJson(enemy3Stats));
            else
                sw.WriteLine("");
        }
        location.loaded = true;
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            TryGetCombatantsStats();

            using (StreamReader sr = File.OpenText(saveFilePath))
            {
                bool usePosition = location.usePosition;  //keep usePosition property as the scene demands
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), location);                
                location.usePosition = usePosition;  //restore usePosition property after load
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), playerStats);
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), quests);
                //JsonUtility.FromJsonOverwrite(sr.ReadLine(), inventory);
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), npcDict);
                npcDict.SetAll();

                string minionLine = sr.ReadLine();
                if (minionLine != "" && minionStats != null)
                    JsonUtility.FromJsonOverwrite(minionLine, minionStats);

                string enemy1Line = sr.ReadLine();
                if (enemy1Line != "" && enemy1Stats != null)
                    JsonUtility.FromJsonOverwrite(enemy1Line, enemy1Stats);
                
                string enemy2Line = sr.ReadLine();
                if (enemy2Line != "" && enemy2Stats != null)
                    JsonUtility.FromJsonOverwrite(enemy2Line, enemy2Stats);

                string enemy3Line = sr.ReadLine();
                if (enemy3Line != "" && enemy3Stats != null)
                    JsonUtility.FromJsonOverwrite(enemy3Line, enemy3Stats);
            }

            LoadNPCs();
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
        location = playerDataObj.AddComponent<PlayerLocation>() as PlayerLocation;
        playerStats = playerDataObj.AddComponent<Stats>() as Stats;
        quests = playerDataObj.AddComponent<Quests>() as Quests;
        //inventory = playerDataObj.AddCompoenent<Inventory>() as Inventory;
    }

    public void NewSave()
    {
        AddPlayerSaveComponents();

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
        minion = GameObject.Find("Minion");
        if (minion)
            minionStats = minion.GetComponent<Stats>();  //if the minion is active, get its stats script
        else
            minionStats = null;  //if the minion doesn't exist, then no need to save its stats

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
        if (location.inBattle)
            return "Battle";
        else
            return location.scene;
    }
}
