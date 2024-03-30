const express = require('express');
const webSocket = require('ws');
const bodyParser = require('body-parser');
const url = require('url');
const app = express();
app.use(express.json());
const server = require('http').createServer(app);
const wss = new webSocket.Server({ server });

var count = 0;

let RoomMap = new Map();

function AddPlayer(roomId, _name, _ws, playerId) {
    if (!RoomMap.has(roomId)) {
        RoomMap.set(roomId, new Map());
    }
    let group = RoomMap.get(roomId);
    group.set(playerId, { name: _name, ws: _ws })
}

function RemovePlayer(roomId, playerId) {
    if (RoomMap.has(roomId)) {
        let group = RoomMap.get(roomId);
        group.delete(playerId);
        if (group.size == 0) {
            RoomMap.delete(roomId);
        }
    }
}

wss.on('connection', (ws,req) => {
    try {
        if (req) {
            const parsedUrl = url.parse(req.url, true);
            const roomId = parsedUrl.query.roomId;
            const playerId = parsedUrl.query.playerId;
            const name = parsedUrl.query.name;
            if (roomId && playerId && name) {
                ws.roomId = roomId;
                ws.playerId = playerId;
                AddPlayer(roomId, name, ws, playerId);
            }
        }
        ws.on('message', (message) => {
            const userMsg = JSON.parse(message);
            let group = RoomMap.get(ws.roomId);
            for (let [key, value] of group) {
                if (key != ws.playerId) {
                    userMsg.playerId = ws.playerId;
                    userMsg.name = value.name;
                    userMsg.type = "message";
                    value.ws.send(JSON.stringify(userMsg));
                    setTimeout(() => {
                        
                    }, 100000);
                }
            }
        });
        ws.on("close", () => {
            for(let [key, value] of RoomMap.get(ws.roomId)){
                if(key != ws.playerId){
                    console.log("connection closed");
                    value.ws.send("{type: 'remove', playerId:"+ws.playerId+"}");
                }
            }
            RemovePlayer(ws.roomId, ws.playerId);
        })
    }
    catch (error) {
        console.log("ws err: " + error);
        let msg = {
            type: "error"
        }
        ws.send(JSON.stringify({type: "error"}));
    }
})

app.get("/", (req, res) => {
    res.send("hello");
})

server.listen(4000, () => {
    console.log("server started at port 4000");
})