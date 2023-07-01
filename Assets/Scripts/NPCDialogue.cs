using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public bool enableDialogue = true;
    public bool readyDialogue = false;
    public bool inDialogue = false;

    public float dialogProgressTime = 0;

    //public Dictionary<string, string> dialogueOptions = new Dictionary<string, string>();
    public List<string> dialogue = new List<string>();
    public int dialogueItem = 0;
    
    private PlayerController pController;
    private TMP_Text dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        dialogueText = GameObject.Find("Canvas").transform.Find("Dialogue").gameObject.GetComponent<TMP_Text>();
        dialogProgressTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableDialogue)  //if dialogue is enabled
        {
            if (pController.readyToSpeak && Time.realtimeSinceStartup - dialogProgressTime > 0.25f)
            {
                if (!pController.movementLocked && readyDialogue)  //if the player is ready to speak and the NPC is ready to speak
                {
                    inDialogue = true;
                    pController.movementLocked = true;
                    //show dialogue
                    //dialogueText.gameObject.transform.position = transform.position;
                    dialogueText.text = dialogue[dialogueItem];
                }
                else
                {
                    if (dialogueItem + 1 < dialogue.Count) //if the NPC has more lines to say
                    {
                        dialogueItem++;
                        dialogueText.text = dialogue[dialogueItem];
                    }
                    else
                    {
                        //the NPC is out of things to say
                        inDialogue = false;
                        pController.movementLocked = false;
                        //hide dialogue
                        dialogueText.text = "";
                    }
                }
                dialogProgressTime = Time.realtimeSinceStartup;
            }
        }
    }
}
