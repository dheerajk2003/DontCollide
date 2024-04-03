using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class DataScript : MonoBehaviour
{
    public static WebSocket ws;
    public static Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    public static HashSet<int> removedEnimies = new HashSet<int>();
    public TMP_Text txtHealth;
    public TMP_Text txtScore;
    public static int bulletForce = 16;
    public static int health = 3;
    public static int score = 0;
    public static int[] ranPos = {-4,4,-10,10};

    void Awake(){
        health = 3;
    }

    void FixedUpdate(){
        txtHealth.text = health.ToString();
        txtScore.text = score.ToString();
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
    }

    public static void sendDelete(int pid)
    {
        ws.Send(JsonConvert.SerializeObject(new {type="remove", playerId= pid}));
    }

    public static void sendBullet(float left, float top, Transform transform){
        try{
            float rotation = transform.rotation.eulerAngles.z;
            ws.Send(JsonConvert.SerializeObject(new {type="bullet", left=left, top=top, rotation=rotation}));
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
    }

    public static void sendHealth(GameObject gameObject, int health){
        // foreach(KeyValuePair<int, GameObject> temp in enimies){}
        int key = enimies.FirstOrDefault(x => x.Value == gameObject).Key;
        ws.Send(JsonConvert.SerializeObject(new {type="health", health=health, playerId= key}));
    }

    public static void clearAll(){
        ws = null;
        enimies.Clear();
        removedEnimies.Clear();
    }
}
