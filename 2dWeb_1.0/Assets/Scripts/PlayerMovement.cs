using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    float movX, movY;
    Rigidbody2D rb;
    WScon wsc = new WScon();
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(movX, movY) * Speed;
        if(Input.GetKeyDown(KeyCode.Space)){
            wsc.sendData("hello server2");
        }
    }
}
