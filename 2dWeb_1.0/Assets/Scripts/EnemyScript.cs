using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyScript : MonoBehaviour
{
    public TMP_Text txtName;
    public TMP_Text txtHealth;
    public float top;
    public float left;
    public int health = 3;
    public int damage = 1;
    public string ename = "name";
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
        txtHealth.text = health.ToString();
        txtName.text = ename;
        switch(health){
            case 1:
                txtHealth.color = Color.red;
                break;
            case 2:
                txtHealth.color = Color.green;
                break;
            case 3:
                txtHealth.color = Color.blue;
                break;
        }
        Debug.Log(rb.position.x + "  " + rb.position.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "bullet")
        {
            health -= damage;
            Debug.Log(health);
            DataScript.sendHealth(gameObject,health);
        }
    }
}
