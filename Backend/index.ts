const express = require('express');
const webSocket = require('ws');
const app = express();
const server = require('http').createServer(app);
const wss = new webSocket.Server({server});

var count = 0;

wss.on('connection', (ws,req) => {
    if(req){
        ws.roomId = req.url;
    }
    if(!ws.playerId){
        ws.playerId = count;
        count++;
    }
    console.log("a new connection " + ws.roomId);
    ws.on('message',(message) => {
        const userMsg = JSON.parse(message);
        wss.clients.forEach(client => {
            if (client !== ws && client.readyState === webSocket.OPEN && client.roomId === ws.roomId) {
                userMsg.playerId = ws.playerId;
                userMsg.count = count;
                client.send(JSON.stringify(userMsg));
            }
        });
        console.log(`${userMsg.top} and ${userMsg.left}`);
    });
})

app.get("/",(req,res) => {
    res.send("hello");
})

server.listen(4000, () => {
    console.log("server started at port 4000");
})