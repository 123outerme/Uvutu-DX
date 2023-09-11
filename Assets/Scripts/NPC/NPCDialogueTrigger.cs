using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    private NPCDialogue dialogue = null;

    // Start is called before the first frame update
    void Start()
    {
        GetDialogueScript();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetDialogueScript()
    {
        if (dialogue == null)
            dialogue = transform.parent.gameObject.GetComponent<NPCDialogue>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GetDialogueScript();
        if (dialogue.enableDialogue)
            dialogue.readyDialogue = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        GetDialogueScript();
        if (dialogue.enableDialogue)
        {
            dialogue.readyDialogue = false;
            dialogue.HideButtonPanel();
        }
    }
}
