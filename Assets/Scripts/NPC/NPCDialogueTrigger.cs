using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.parent.gameObject.GetComponent<NPCDialogue>().enableDialogue)
            transform.parent.gameObject.GetComponent<NPCDialogue>().readyDialogue = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (transform.parent.gameObject.GetComponent<NPCDialogue>().enableDialogue)
            transform.parent.gameObject.GetComponent<NPCDialogue>().readyDialogue = false;
    }
}
