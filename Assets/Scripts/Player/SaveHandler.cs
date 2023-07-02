using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveHandler : MonoBehaviour
{
    public string saveDirectory = "save";
    public string saveFile = "save1.txt";
    
    public GameObject playerDataObj;

    private PlayerStats stats;
    private Quests quests;
    //private Inventory inventory;
    private NPCDict npcDict;
    
    private string saveFilePath;

    // Start is called before the first frame update
    void Awake()
    {
        stats = playerDataObj.GetComponent<PlayerStats>();
        quests = playerDataObj.GetComponent<Quests>();
        //inventory = playerDataObj.GetComponent<Inventory>();
        npcDict = new NPCDict();
        ComputeSavePath();

        Load();
    }

    public void Save()
    {
        //for all NPCs in the current scene
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject npcObj in npcs)
        {
            NPCStats stats = npcObj.GetComponent<NPCStats>();
            stats.SaveNPCState();
            npcDict.Set(stats.state);  //set state of that NPC
        }

        //create save folder
        Directory.CreateDirectory(saveDirectory);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            sw.WriteLine(JsonUtility.ToJson(stats));
            sw.WriteLine(JsonUtility.ToJson(quests));
            //sw.WriteLine(JsonUtility.ToJson(inventory));
            npcDict.GetAll();
            sw.WriteLine(JsonUtility.ToJson(npcDict));
        }
        stats.loaded = true;
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            using (StreamReader sr = File.OpenText(saveFilePath))
            {
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), stats);
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), quests);
                //JsonUtility.FromJsonOverwrite(sr.ReadLine(), inventory);
                JsonUtility.FromJsonOverwrite(sr.ReadLine(), npcDict);
                npcDict.SetAll();
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
            NPCStats stats = npcObj.GetComponent<NPCStats>();
            stats.LoadNPCState(state);
        }
    }

    public void NewSave()
    {
        stats = playerDataObj.AddComponent<PlayerStats>() as PlayerStats;
        quests = playerDataObj.AddComponent<Quests>() as Quests;
        //inventory = playerDataObj.AddCompoenent<Inventory>() as Inventory;
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
}
