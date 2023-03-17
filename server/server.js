const WebSocket = require('ws');
const messageHandler = require('./App/messageHandler');

const server = new WebSocket.Server({
    host: 'localhost',
    port: 8080
});

console.log('Starting WebSocket server...');
server.on('connection', (socket) => {
    console.log('Client connected');
    socket.send('{"type":"userData", "content":"lol"}');
    socket.on('message', (message) => {
      console.log(`Received message: ${message}`);
    });
  
    socket.on('close', () => {
      console.log('Client disconnected');
    });
});
