using System;
using WebSocketSharp;

public class WScon{
    WebSocket ws;
    public WScon(){
        ws = new WebSocket("ws://localhost:4000");
        ws.OnMessage += (sender, e) => {
            Console.WriteLine("Received: " + e.Data);
        };
        ws.Connect();
    }
    public void sendData(string data){
        if(ws != null){
            ws.Send(data);
        }
        else{
            Console.WriteLine("ws is null");
        }
    }
}