using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

public class DataScript : MonoBehaviour
{
    public static WebSocket ws;
    public static Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    public static HashSet<int> removedEnimies = new HashSet<int>();

    public static int bulletForce = 20;

    public static int health = 3;

    public static void sendDelete(int pid)
    {
        ws.Send(JsonConvert.SerializeObject(new {type="remove", playerId= pid}));
    }

    public static void sendBullet(float left, float top, Transform transform){
        try{
            float rotation = transform.rotation.eulerAngles.z;
            Debug.Log("sending rotation : " + rotation);
            ws.Send(JsonConvert.SerializeObject(new {type="bullet", left=left, top=top, rotation=rotation}));
        }
        catch(Exception e){
            Debug.Log(e.Message);
        }
    }

    public static void sendHealth(GameObject gameObject, int health){
        // foreach(KeyValuePair<int, GameObject> temp in enimies){}
        int key = enimies.FirstOrDefault(x => x.Value == gameObject).Key;
        Debug.Log("health in ds = " + health);
        ws.Send(JsonConvert.SerializeObject(new {type="health", health=health, playerId= key}));
    }

    public static void clearAll(){
        ws = null;
        enimies.Clear();
        removedEnimies.Clear();
    }
}
