const WebSocket = require('ws');
const userData = require('./App/userData');

let clients = {};

let message = {
  Type: '',
  Content: ''
};


const server = new WebSocket.Server({
    host: 'localhost',
    port: 8080
});

console.log('Starting WebSocket server...');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (socket, message) => {
      HandleMessage(message)
    });
  
    socket.on('close', () => {
      console.log('Client disconnected');
      for (let userId in clients) {
        if (clients[userId].socket === socket) {
            delete clients[userId];
            break;
        }
      }
    });
});

const HandleMessage = async (data) => {
    message = JSON.parse(data);
    switch(message.Type){
        case "getUserData":
            await getUserData(socket, message.Content);
            break;
        default:
            break;
    }
};

const getUserData = async (socket, Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = await userData.createUserId();
    if(user.username == '') user.username = await userData.createUsername();

    clients[user.userId] = {
      socket: socket,
      userId: user.userId
    };

    let content = JSON.stringify(user);

    message = {
        Type: 'userData',
        Content: content
    }
    
    for (let userId in clients) {
      if (clients[userId].socket === socket) {
          socket.send(JSON.stringify(message));
          break;
      }
    }

    return 0;
};
