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

public class enemyType
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
    public Transform FirePoint;
    public Camera cam;
    Vector2 mousePos;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ws = new WebSocket("ws://localhost:4000?roomId=" + UiScript.RoomId + "&playerId=" + UiScript.PlayerId + "&name=" + UiScript.Name);
        ws.OnMessage += OnWebSocketMessageReceived;
        ws.Connect();
        DataScript.ws = ws;
    }
    void Update()
    {
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        try
        {
            rb.velocity = new Vector2(movX, movY) * Speed;

            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

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

    void OnWebSocketMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            enemyType eData = JsonConvert.DeserializeObject<enemyType>(e.Data);
            if(eData.type != "message"){
                Debug.Log("not msg " + eData.playerId + " " + UiScript.PlayerId);
            }
            if (eData.type == "error")
            {
                ws.Close();
                SceneManager.LoadScene("LoginScene");
            }

            else if (eData.type == "remove")
            {
                Debug.Log("inside remove" + eData.playerId + " " + UiScript.PlayerId);
                DesEnemy(eData);

            }

            else if(eData.type == "deMe"){
                Debug.Log("inside remove if");
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                SceneManager.LoadScene("LoginScene");
                });
            }

            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (DataScript.enimies.ContainsKey(eData.playerId))
                    {
                        EnemyScript escript = DataScript.enimies[eData.playerId].GetComponent<EnemyScript>();
                        if (escript != null)
                        {
                            escript.top = eData.top;
                            escript.left = eData.left;
                        }

                    }
                    else if (!DataScript.removedEnimies.Contains(eData.playerId))
                    {
                        var newEnemy = Instantiate(enemyPrefab, new Vector2(eData.left, eData.top), Quaternion.identity);
                        DataScript.enimies.Add(eData.playerId, newEnemy);
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

    void DesEnemy(enemyType eData)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if(DataScript.enimies.ContainsKey(eData.playerId))
            {
                Destroy(DataScript.enimies[eData.playerId].gameObject);
                DataScript.removedEnimies.Add(eData.playerId);
                DataScript.enimies.Remove(eData.playerId);
            }
            else{
                Debug.Log("not found" + eData.playerId);
            }
        });
    }

}