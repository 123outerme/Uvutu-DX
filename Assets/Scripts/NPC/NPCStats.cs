using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCState
{
    public Vector3 position;
    public bool enableMovement;
    public bool startMovement;
    public int step;
    public int frame;
    public string name;

    public NPCState(Vector3 pos, bool enableMove, bool startMove, int curStep, int curFrame, string npcName)
    {
        position = pos;
        enableMovement = enableMove;
        startMovement = startMove;
        step = curStep;
        frame = curFrame;
        name = npcName;
    }
}

public class NPCStats : MonoBehaviour
{
    public bool saveStatus = true;
    public NPCState state = new NPCState(new Vector3(0,0,0), false, false, 0, 0, "");

    private NPCMovement movement;
    private NPCDialogue dialogue;

    private SaveHandler saver;

    public void Start()
    {
        if (movement == null)
            movement = gameObject.GetComponent<NPCMovement>();
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
        //SaveNPCState();
    }

    public void SaveNPCState()
    {
        state = new NPCState(transform.position, movement.enableMovement, movement.startMovement, movement.step, movement.frame, gameObject.name);
    }

    public void LoadNPCState(NPCState s)
    {
        if (s != null)
        {
            if (movement == null)
                movement = gameObject.GetComponent<NPCMovement>();
            
            state = s;
            transform.position = state.position;
            movement.enableMovement = state.enableMovement;
            movement.step = state.step;
            movement.frame = state.frame;

            movement.lastMinusNext = movement.posSteps[movement.step] - state.position;
            movement.startMovement = state.startMovement;
            Debug.Log("given state is not null.");
        }
        else
        {
            Debug.Log("given state is null!!");  //This triggers on first load. Why?
        }
    }
}
