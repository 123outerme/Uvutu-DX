using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject overworldEnemyPrefab;
    public Combatant[] enemyOptions;
    public float[] enemyChances;

    public bool enemyIsSpawned = false;

    public bool enemyCanSpawn = false;

    private MapLoader mapLoader;

    // Start is called before the first frame update
    void Start()
    {
        if (mapLoader == null)
            mapLoader = GameObject.Find("MapLoader").GetComponent<MapLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCanSpawn && !enemyIsSpawned && mapLoader != null && mapLoader.navMeshBuilt)
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
                enemyIsSpawned = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        enemyCanSpawn = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemyCanSpawn = false;
    }
}
