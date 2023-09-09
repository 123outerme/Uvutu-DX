using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class OverworldEnemy : MonoBehaviour
{
    public Combatant combatant;
    public EnemySpawner parentSpawner;

    public Vector3 homePosition;

    private UnityEngine.AI.NavMeshAgent agent = null;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateAgent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateAgent()
    {
        if (agent != null)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    public void LoadEnemyFromCombatant()
    {
        //TODO
        Image spr = GetComponent<Image>();
        spr.sprite = combatant.sprite;
    }

    public void TargetPlayer()
    {
        //TODO
        UpdateAgent();
    }

    public void TargetHome()
    {
        //TODO
        UpdateAgent();
    }
}
