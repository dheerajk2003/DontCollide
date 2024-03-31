using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyScript : MonoBehaviour
{
    public float top;
    public float left;
    public int health = 3;
    public int damage = 1;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        try
        {
            if (health <= 0)
            {
                int id = 0;
                foreach (KeyValuePair<int, GameObject> temp in DataScript.enimies)
                {
                    if (temp.Value == gameObject)
                    {
                        id = temp.Key;
                    }
                }
                DataScript.sendDelete(id);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    void FixedUpdate()
    {
        rb.position = new Vector2(left, top);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "bullet")
        {
            health -= damage;
        }
    }
}
