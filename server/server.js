const WebSocket = require('ws');
const messageHandler = require('./App/messageHandler');

const server = new WebSocket.Server({
    host: 'localhost',
    port: 8080
});

console.log('Starting WebSocket server...');

server.on('connection', (socket) => {
    console.log('Client connected');
    console.log(messageHandler.startupMessage);
    socket.send(JSON.stringify(messageHandler.startupMessage));

    socket.on('message', async (message) => {
      messageHandler.HandleMessage(message)
    });
  
    socket.on('close', () => {
      console.log('Client disconnected');
    });
});
