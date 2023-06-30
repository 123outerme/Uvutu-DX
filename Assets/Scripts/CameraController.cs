using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePlayerDist = player.transform.position - transform.position;

        if (Mathf.Abs(relativePlayerDist.x) > 10)
            transform.Translate(10 * Mathf.Sign(relativePlayerDist.x), 0, 0);

        if (Mathf.Abs(relativePlayerDist.y) > 10)
            transform.Translate(0, 10 * Mathf.Sign(relativePlayerDist.y), 0);
    }
}
