using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject resumeButton;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private SaveHandler saver;

    // Start is called before the first frame update
    void Start()
    {
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();

        if (!saver.SavesExist())  //if no saves exist
            resumeButton.SetActive(false);  //hide the resume button
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Overworld"));
    }

    public void NewGame()
    {
        saver.NewSave();  //save before loading a new game, thereby creating a new game file
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Overworld"));
    }

    public void OpenSettings()
    {
        //TODO
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
