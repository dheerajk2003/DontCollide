using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Runtime.Serialization;

class enemyType{
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
    Dictionary<int,GameObject> enimies = new Dictionary<int,GameObject>();
    Dictionary<int,enemyType> enemiesType = new Dictionary<int,enemyType>();
    // Start is called before the first frame update
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
        
        StartCoroutine(SdCouroutine());
    }

    void OnWebSocketMessageReceived(object sender, MessageEventArgs e){
        try{
                var eData = JsonConvert.DeserializeObject<enemyType>(e.Data);

                if(enemiesType.ContainsKey(eData.playerId)){
                    enemiesType[eData.playerId] = eData;
                }
                else{
                    enemyType et = new enemyType();
                    et.top = eData.top;
                    et.left =eData.left;
                    et.playerId = eData.playerId;
                    enemiesType.Add(eData.playerId, et);
                }

                // if(enimies.ContainsKey(eData.playerId)){
                //     var enemy = enimies[eData.playerId];
                //     // Update the enemy's position
                //     enemiesType[eData.playerId] = eData;
                // }
                // else{
                //     var newEnemy = Instantiate(enemyPrefab, new Vector2(eData.left, eData.top),Quaternion.identity);
                //     enimies.Add(eData.playerId, newEnemy);
                //     var newEnemyType = new enemyType();
                //     newEnemyType.top = eData.top;
                //     newEnemyType.left = eData.left;
                //     newEnemyType.playerId = eData.playerId;
                //     enemiesType.Add(eData.playerId, newEnemyType);
                // }
            }
            catch(System.Exception ex){
                Debug.Log(ex.Message);
            }
    }

    IEnumerator SdCouroutine(){
        yield return new WaitForSeconds(0.03f);
        if(ws !=null && ws.ReadyState == WebSocketState.Open){
            ws.Send(JsonConvert.SerializeObject(new {top = rb.position.y, left = rb.position.x}));
        }
        foreach(var enemy in enemiesType){
            // enemy.Value.transform.position = new Vector2(enemiesType[enemy.Key].left, enemiesType[enemy.Key].top);
            if(enimies.ContainsKey(enemy.Key)){
                enimies[enemy.Key].transform.position = new Vector2(enemy.Value.left, enemy.Value.top);
            }
            else{
                var newEnemy = Instantiate(enemyPrefab, new Vector2(enemy.Value.left, enemy.Value.top),Quaternion.identity);
                enimies.Add(enemy.Key, newEnemy);
            }
        }
    }
}
