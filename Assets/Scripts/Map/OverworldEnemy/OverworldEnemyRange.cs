using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyRange : MonoBehaviour
{
    private OverworldEnemy parent = null;

    // Start is called before the first frame update
    void Start()
    {
        GetParentScript();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetParentScript()
    {
        if (parent == null)
            parent = transform.parent.gameObject.GetComponent<OverworldEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GetParentScript();
        parent.TargetPlayer(); //set enemy to nav towards player
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GetParentScript();
        parent.TargetHome(); //set enemy to nav around home
    }
}
