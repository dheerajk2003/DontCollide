using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using PimDeWitte.UnityMainThreadDispatcher;

class enemyType
{
    public float top;
    public float left;
    public int playerId;
    public string type;
    public string name;
}

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public GameObject enemyPrefab;
    float movX, movY;
    Rigidbody2D rb;
    WebSocket ws;
    Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ws = new WebSocket("ws://localhost:4000?roomId=" + UiScript.RoomId + "&playerId=" + UiScript.PlayerId + "&name=" + UiScript.Name);
        ws.OnMessage += OnWebSocketMessageReceived;
        ws.Connect();
    }
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
            enemyType eData = JsonConvert.DeserializeObject<enemyType>(e.Data);
            if (eData.type == "error")
            {
                SceneManager.LoadScene("LoginScene");
            }

            else if (eData.type == "remove")
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Destroy(enimies[eData.playerId].gameObject);
                    enimies.Remove(eData.playerId);
                    Debug.Log("Deleted");
                });
            }
            
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (enimies.ContainsKey(eData.playerId))
                    {
                        EnemyScript escript = enimies[eData.playerId].GetComponent<EnemyScript>();
                        if (escript != null)
                        {
                            escript.top = eData.top;
                            escript.left = eData.left;
                        }

                    }
                    else
                    {
                        var newEnemy = Instantiate(enemyPrefab, new Vector2(eData.left, eData.top), Quaternion.identity);
                        enimies.Add(eData.playerId, newEnemy);
                    }
                });
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("application is getting closed");
        ws.Close();
    }


    void FixedUpdate()
    {
        try
        {
            if (ws != null && ws.ReadyState == WebSocketState.Open)
            {
                ws.Send(JsonConvert.SerializeObject(new { top = rb.position.y, left = rb.position.x }));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}