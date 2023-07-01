using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementTrigger : MonoBehaviour
{
    public bool enableTrigger = true;

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
        if (transform.parent.gameObject.GetComponent<NPCMovement>().enableMovement)
            transform.parent.gameObject.GetComponent<NPCMovement>().startMovement = true;
    }
}
