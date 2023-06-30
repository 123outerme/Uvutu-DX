using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 8;
    public bool lastFlip = false;
    
    private SpriteRenderer parentRenderer;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        parentRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(x, y).normalized * speed;
        
        if (x != 0 && (x < 0 ^ lastFlip)) //x position is changing and (x < 0 LOGICAL XOR lastFlip == true)
        {
            lastFlip = (x < 0);
            parentRenderer.flipX = lastFlip;
        }
    }
}
