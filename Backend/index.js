const express = require('express');
const webSocket = require('ws');
const app = express();
const server = require('http').createServer(app);
const wss = new webSocket.Server({server});

wss.on("connection", (ws) => {
    ws.on("message", (message) => {
        console.log("received msg : " , message.toString());
        ws.send(message.toString());
    })
})

app.get("/",(req,res) => {
    res.send("hello");
})

server.listen(4000, () => {
    console.log("server started at port 4000");
})