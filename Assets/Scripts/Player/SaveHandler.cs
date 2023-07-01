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
    private string saveFilePath;

    // Start is called before the first frame update
    void Start()
    {
        stats = playerDataObj.GetComponent<PlayerStats>();
        quests = playerDataObj.GetComponent<Quests>();
        //inventory = playerDataObj.GetComponent<Inventory>();
        ComputeSavePath();
    }

    public void Save()
    {
        //create save folder
        Directory.CreateDirectory(saveDirectory);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            sw.WriteLine(JsonUtility.ToJson(stats));
            sw.WriteLine(JsonUtility.ToJson(quests));
            //sw.WriteLine(JsonUtility.ToJson(inventory));
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
            }
        }
    }

    public void NewSave()
    {
        stats = playerDataObj.AddComponent<PlayerStats>() as PlayerStats;
        quests = playerDataObj.AddComponent<Quests>() as Quests;
        //inventory = playerDataObj.AddCompoenent<Inventory>() as Inventory;
        Save();
    }

    public bool SavesExist()
    {
        ComputeSavePath();
        Debug.Log(File.Exists(saveFilePath) + " : " + saveFilePath);
        return File.Exists(saveFilePath);
    }

    private void ComputeSavePath()
    {
        string[] paths = new string[] {saveDirectory, saveFile};

        saveFilePath = Path.Combine(paths);
    }
}
