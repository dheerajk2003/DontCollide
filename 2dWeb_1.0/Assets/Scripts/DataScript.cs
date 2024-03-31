using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

public class DataScript : MonoBehaviour
{
    public static WebSocket ws;
    public static Dictionary<int, GameObject> enimies = new Dictionary<int, GameObject>();
    public static HashSet<int> removedEnimies = new HashSet<int>();

    public static void sendDelete(int pid)
    {
        ws.Send(JsonConvert.SerializeObject(new {type="remove", playerId= pid}));
    }
}
