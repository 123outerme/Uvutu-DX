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
    public float homeMonitorRadius = 3.0f;
    public Vector3 nearbyHomePos;

    private NavMeshAgent agent = null;

    private bool targetPlayer = false;
    private GameObject player = null;

    private float newHomeSetTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            //Debug.Log(transform.position + " / " + nearbyHomePos + " = " + (transform.position - nearbyHomePos).magnitude);
            if ((transform.position - nearbyHomePos).magnitude < 0.75f)
            {
                //if the agent has reached its home-patrol target, get a new patrol position
                GetNewNearbyHomePos();
                newHomeSetTime = Time.realtimeSinceStartup;
            }

            if (newHomeSetTime != 0.0f && Time.realtimeSinceStartup - newHomeSetTime > 1.0f)
            {
                //if the agent has sat at its patrol spot for more than 1.0 seconds, set the new destination
                agent.SetDestination(nearbyHomePos);
                newHomeSetTime = 0.0f;
            }
        }
    }

    private void UpdateAgent()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        if (player == null)
            player = GameObject.Find("Player");
    }

    public void LoadEnemyFromCombatant()
    {
        UpdateAgent();
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        spr.sprite = combatant.sprite;
        TargetHome();
    }

    public void TargetPlayer()
    {
        UpdateAgent();
        targetPlayer = true;
    }

    public void TargetHome()
    {
        UpdateAgent();
        targetPlayer = false;
        GetNewNearbyHomePos();
        agent.SetDestination(nearbyHomePos);
    }

    private void GetNewNearbyHomePos()
    {
        Vector2 offset = Random.insideUnitCircle;
        nearbyHomePos = homePosition + (new Vector3(offset.x, offset.y, 0.0f) * homeMonitorRadius);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //start battle!
        GameObject loader = GameObject.Find("SceneLoader");
        if (loader != null)
            loader.GetComponent<SceneLoader>().LoadBattle(combatant);
    }
}
