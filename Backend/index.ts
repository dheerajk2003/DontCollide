const express = require('express');
const webSocket = require('ws');
const bodyParser = require('body-parser');
const url = require('url');
const path = require('path');
const app = express();
app.use(express.json());
const server = require('http').createServer(app);
const wss = new webSocket.Server({ server });

app.use('/Build', express.static(path.join(__dirname,'/build1/Build')));
app.use('/TemplateData', express.static(path.join(__dirname,'/build1/TemplateData')));

let RoomMap = new Map();

function AddPlayer(roomId, _name, _ws, playerId) {
    if (!RoomMap.has(roomId)) {
        RoomMap.set(roomId, new Map());
    }
    let group = RoomMap.get(roomId);
    group.set(playerId, { name: _name, ws: _ws })
}

function RemovePlayer(roomId, pid) {
    let group = RoomMap.get(roomId);
    if (group != null && group != undefined && group.size >= 1) {
        // console.log("inside remove player" + group.get(pid));
        group.delete(pid);
        if (group.size == 0) {
            RoomMap.delete(roomId);
        }
    }

}

wss.on('connection', (ws, req) => {
    console.log("new connection");
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
            if (group != null && group != undefined && group.size >= 1) {
                // if(userMsg.type != "message"){
                //     console.log("Type: " + userMsg.type);
                // }
                if (userMsg.type == "remove") {
                    for (let [key, value] of group) {
                        if (userMsg.playerId == key) {
                            // console.log("delete me here " + userMsg.playerId + " " + key);
                            value.ws.send(JSON.stringify({ type: "deMe" }));
                            RemovePlayer(ws.roomId, key);
                        }
                        else{
                            // console.log("delete another " + userMsg.playerId + " " + key);
                            value.ws.send(JSON.stringify({ type: "remove", playerId: userMsg.playerId }));
                        }
                    }
                }
                else if(userMsg.type == "health"){
                    let group = RoomMap.get(ws.roomId);
                    if(group != null && group != undefined && group.size >= 1){
                        for (let [key, value] of group) {
                            value.ws.send(JSON.stringify(userMsg));
                        }
                    }
                }
                else {
                    for (let [key, value] of group) {
                        if (key != ws.playerId) {
                            userMsg.playerId = ws.playerId;
                            userMsg.name = value.name;
                            value.ws.send(JSON.stringify(userMsg));
                        }
                    }
                }
            }
        });
        ws.on("close", () => {
            let group = RoomMap.get(ws.roomId);
            if (group != null && group != undefined && group.size >= 1) {
                for (let [key, value] of group) {
                    value.ws.send(JSON.stringify({ type: "remove", playerId: ws.playerId }));
                }
            }
            RemovePlayer(ws.roomId, ws.playerId);
        })
    }
    catch (error) {
        console.log("ws err: " + error);
        ws.send(JSON.stringify({ type: "error" }));
    }
})

app.get("/", (req, res) => {
    res.sendFile(__dirname + "/build1/index.html");
})

server.listen(4000, () => {
    console.log("server started at port 4000");
})