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
            console.log(req.url);
            const parsedUrl = url.parse(req.url, true);
            const roomId = parsedUrl.query.roomId;
            const playerId = parsedUrl.query.playerId;
            console.log(roomId, playerId);
            if (roomId && playerId) {
                let group = RoomMap.get(roomId);
                ws.roomId = roomId;
                ws.playerId = playerId;
                let plyr = group.get(playerId);
                plyr.ws = ws;
                plyr.ws.send("{playerId: 'connected'}");
            }
        }
        // if(!ws.playerId){
        //     ws.playerId = count;
        //     count++;
        // }
        // console.log("a new connection " + ws.roomId);
        ws.on('message', (message) => {
            const userMsg = JSON.parse(message);
            // wss.clients.forEach(client => {
            //     if (client !== ws && client.readyState === webSocket.OPEN && client.roomId === ws.roomId) {
            //         userMsg.playerId = ws.playerId;
            //         userMsg.count = count;
            //         client.send(JSON.stringify(userMsg));
            //     }
            // });
            let group = RoomMap.get(ws.roomId);
            for (let [key, value] of group) {
                if (key != ws.playerId) {
                    console.log(key, value.name);
                    // userMsg.playerId = ws.playerId;
                    // userMsg.name = value.name;
                    // let g = RoomMap.get(key);
                    // console.log(g.get(userMsg.playerId));
                    // console.log(value.ws.playerId);
                    // value.ws.send(JSON.stringify(userMsg));
                    setTimeout(() => {
                        
                    }, 100000);
                }
            }
            // console.log(`${userMsg.top} and ${userMsg.left}`);
        });
        ws.on("close", () => {
            RemovePlayer(ws.roomId, ws.playerId);
        })
    }
    catch (error) {
        console.log("ws err: " + error);
    }
})

app.get("/", (req, res) => {
    res.send("hello");
})

app.post("/newCon", (req, res) => {
    try {
        const data = req.body;
        AddPlayer(data.roomId, data.name, null, data.playerId);
        console.log(data);
        res.sendStatus(200);
    }
    catch(error) {
        console.log(error);
        res.sendStatus(500);
    }
})

server.listen(4000, () => {
    console.log("server started at port 4000");
})