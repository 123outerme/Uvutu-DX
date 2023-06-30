using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveHandler : MonoBehaviour
{
    public string saveDirectory = "save";
    public string saveFile = "save1.txt";
    
    public GameObject playerStatsParent;

    private PlayerStats stats;
    private string saveFilePath;

    // Start is called before the first frame update
    void Start()
    {
        stats = playerStatsParent.GetComponent<PlayerStats>();
        ComputeSavePath();
    }

    public void Save()
    {
        //create save folder
        Directory.CreateDirectory(saveDirectory);

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(saveFilePath))
        {
            sw.WriteLine(JSONFromStats(stats));
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
            }
        }
    }

    public void NewSave()
    {
        stats = playerStatsParent.AddComponent<PlayerStats>() as PlayerStats;
        Save();
    }

    public bool SavesExist()
    {
        ComputeSavePath();
        Debug.Log(File.Exists(saveFilePath) + " : " + saveFilePath);
        return File.Exists(saveFilePath);
    }

    private string JSONFromStats(PlayerStats pStats)
    {
        return JsonUtility.ToJson(pStats);
    }

    private void ComputeSavePath()
    {
        string[] paths = new string[] {saveDirectory, saveFile};

        saveFilePath = Path.Combine(paths);
    }
}
