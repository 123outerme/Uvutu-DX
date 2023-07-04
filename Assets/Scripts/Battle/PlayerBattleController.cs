using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))  //pause menu, default ESC
        {
            GameObject loader = GameObject.Find("SceneLoader"); //get the scene loader
            if (loader != null)
            {
                SceneLoader loadScript = loader.GetComponent<SceneLoader>();  //get the loader script
                if (loadScript != null)
                    loadScript.LoadPauseMenu();  //pause the game
            }
        }

        if (Input.GetButtonUp("Fire2"))
        {
            
            GameObject loader = GameObject.Find("SceneLoader");
            if (loader != null)
            {
                SceneLoader loadScript = loader.GetComponent<SceneLoader>();
                if (loadScript != null)
                {
                    GetComponent<PlayerLocation>().exitingBattle = true;
                    loadScript.ResumeGame();
                }
            }
        }
    }
}
