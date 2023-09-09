using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject overworldEnemyPrefab;
    public Combatant[] enemyOptions;
    public float[] enemyChances;

    public float spawnChancePerFrame = 0.01f;

    public bool enemyIsSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyIsSpawned && Random.Range(0.0f, 1.0f) <= spawnChancePerFrame)
        {
            int enemyIndex = WeightedRandomChoice.Pick(enemyChances);
            
            if (enemyIndex > -1)
            {
                Vector3 enemyPos = new Vector3();
                enemyPos += transform.position;
                GameObject enemy = Instantiate(overworldEnemyPrefab, enemyPos, Quaternion.identity, transform) as GameObject;
                OverworldEnemy enemyScript = enemy.GetComponent<OverworldEnemy>();
                enemyScript.combatant = enemyOptions[enemyIndex];
                enemyScript.parentSpawner = this;
                enemyScript.homePosition = enemyPos;
                enemyScript.LoadEnemyFromCombatant();
            }
        }
    }
}
