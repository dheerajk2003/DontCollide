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

class EnUp{
    public void getEnmy(enemyType eData, GameObject enemy){
        EnemyScript escript = enemy.GetComponent<EnemyScript>();
        if(escript != null){
            escript.top = eData.top;
            escript.left = eData.left;
        }
    }
}

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
    EnUp enup = new EnUp();
    Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    Dictionary<int, enemyType> enemiesType = new Dictionary<int, enemyType>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ws = new WebSocket("ws://localhost:4000?roomId=" + UiScript.RoomId + "&playerId=" + UiScript.PlayerId+ "&name=" + UiScript.Name);
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
            if(eData.type == "error"){
                SceneManager.LoadScene("LoginScene");
            }
            
            if(eData.type == "remove"){
                Debug.Log("got remove request");
                // desEnm = enimies[eData.playerId];
                UnityMainThreadDispatcher.Instance().Enqueue(() => {Destroy(enimies[eData.playerId].gameObject);});
                enemiesType.Remove(eData.playerId);
                enimies.Remove(eData.playerId);
            }

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

    void OnApplicationQuit(){
        Debug.Log("application is getting closed");
        ws.Close();
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
                    // enimies[enemy.Key].transform.position = new Vector2(enemy.Value.left, enemy.Value.top);
                    enup.getEnmy(enemy.Value, enimies[enemy.Key]);
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