using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using System;

class enemyType
{
    public float top;
    public float left;
    public int playerId;
}



public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public GameObject enemyPrefab;
    float movX, movY;
    Rigidbody2D rb;
    WebSocket ws;
    Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    Dictionary<int, enemyType> enemiesType = new Dictionary<int, enemyType>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ws = new WebSocket("ws://localhost:4000/1");
        ws.OnMessage += OnWebSocketMessageReceived;
        ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(movX, movY) * Speed;
    }

    void OnWebSocketMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            var eData = JsonConvert.DeserializeObject<enemyType>(e.Data);

            if (enemiesType.ContainsKey(eData.playerId))
            {
                enemiesType[eData.playerId] = eData;
            }
            else
            {
                enemyType et = new enemyType();
                et.top = eData.top;
                et.left = eData.left;
                et.playerId = eData.playerId;
                enemiesType.Add(eData.playerId, et);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }


    void FixedUpdate()
    {
        try{
            if (ws != null && ws.ReadyState == WebSocketState.Open)
            {
                ws.Send(JsonConvert.SerializeObject(new { top = rb.position.y, left = rb.position.x }));
            }

            updateEnemies();
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
    }

    public void updateEnemies()
    {
        try
        {
            foreach (var enemy in enemiesType)
            {
                if (enimies.ContainsKey(enemy.Key))
                {
                    enimies[enemy.Key].transform.position = new Vector2(enemy.Value.left, enemy.Value.top);
                }
                else
                {
                    var newEnemy = Instantiate(enemyPrefab, new Vector2(enemy.Value.left, enemy.Value.top), Quaternion.identity);
                    enimies.Add(enemy.Key, newEnemy);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
