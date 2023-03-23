const WebSocket = require('ws');
const userData = require('./App/userData');

let clients = {

};

let message = {
  Type: "",
  Content: ""
};

const clientsLoop = async () => {
  while(true){
    for (const userId in clients) {
      console.log(clients[userId]);
    }
    await new Promise(resolve => setTimeout(resolve, 1000));
  }
};

clientsLoop();

const server = new WebSocket.Server({
    host: 'localhost',
    port: 8080
});

console.log('Starting WebSocket server...');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (socket, message) => {
      HandleMessage(socket, message)
    });
  
    socket.on('close', (socket) => {
      console.log('Client disconnected');
      for (let userId in clients) {
        if (clients[userId].socket === socket) {
            delete clients[userId];
            break;
        }
      }
    });
});

const HandleMessage = async (socket, data) => {
    message = JSON.parse(data);
    console.log(message);
    console.log(message.Type);
    switch(message.Type){
        case "getUserData":
            console.log('GetUserData message');
            await getUserData(socket, message.Content);
            break;
        default:
            break;
    }
};

const getUserData = async (Socket, Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = await userData.createUserId();
    if(user.username == '') user.username = await userData.createUsername();

    console.log('Adding new client to dictionary');
    clients[user.userId] = {
      socket: Socket,
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
