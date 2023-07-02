using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class NPCDict
{
    public NPCState[] npcStates;
    private List<NPCState> states;

    public NPCDict()
    {
        npcStates = new NPCState[0];
        states = new List<NPCState>();
    }

    public NPCState Get(string key)
    {
        foreach(NPCState state in states)
        {
            if (state.name == key)
                return state;
        }
        return null;
    }

    public void Set(NPCState s)
    {
        bool found = false;

        for(int i = 0; i < states.Count; i++) 
        {
            if (states[i] != null && s != null && states[i].name == s.name)
            {
                states[i] = s;
                found = true;
            }
        }

        if (!found)
            states.Add(s);
    }

    public NPCState[] GetAll()
    {
        npcStates = states.ToArray();
        return npcStates;
    }

    public void SetAll()
    {
        states = new List<NPCState>(npcStates);
    }
}