using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public List<Vector3> posSteps = new List<Vector3>();
    public List<float> timeSteps = new List<float>();
    public bool loop = false;
    public bool enableMovement = false;
    public bool startMovement = false;

    public int step = 0;
    public int frame = 0;

    public Vector3 lastMinusNext = new Vector3(0,0,0);

    //private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        if (lastMinusNext == new Vector3(0,0,0))
            lastMinusNext = posSteps[0] - transform.position;  //get difference in this position and last for movement
        
        if (posSteps.Count <= 0 && timeSteps.Count != posSteps.Count)
            enableMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableMovement && startMovement)
        {
            //rb.velocity = new Vector2(lastMinusNext.x, lastMinusNext.y) / timeSteps[step];
            
            Vector3 velocity = new Vector3(lastMinusNext.x, lastMinusNext.y, 0) / timeSteps[step];  //units per frame
            transform.position = transform.position + velocity;  //add velocity this frame
            frame++;  //increment framecounter

            if (frame >= timeSteps[step])
            {
                int nextStep = (step + 1) % posSteps.Count;  //calculate next step index
                transform.position = posSteps[step]; //lock in exactly at the desired position
                lastMinusNext = posSteps[nextStep] - posSteps[step];  //intuition states [step] - [nextStep] like in Start(), but idk
                frame = 0;  //reset framecounter

                if (nextStep < step && !loop)  //if movement is about to loop and looping is disabled
                    enableMovement = false;  //stop moving
                else
                    step = nextStep;  //otherwise continue to the next step
            }
        }
    }

    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        enableMovement = false;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        enableMovement = true;
    }
    //*/
}
