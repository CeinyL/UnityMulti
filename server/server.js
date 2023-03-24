const WebSocket = require('ws');
const userData = require('./App/userData');

let clients = {

};

let message = {
  Type: "",
  Content: ""
};

const server = new WebSocket.Server({
  host: 'localhost',
  port: 8080
});

const clientsLoop = async () => {
  while(true){
    for (const userId in clients) {
      console.log(""+clients[userId].userId);
    }
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};

process.argv.forEach(function (val, index, array) {
  if(index==2&&val=="remote")  server.host='192.168.1.12';
  else server.host='localhost';
});


clientsLoop();




console.log('Starting WebSocket server: '+server.host);


server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (data) => {
      HandleMessage(socket, data);
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

const HandleMessage = async (socket, data) => {
    
    try {
      message = JSON.parse(data);
      console.log(message);
      console.log(message.Type);
      switch(message.Type){
        case "getUserData":
            console.log('GetUserData message');
            await getUserData(socket, message.Content);
            break;
        default:
            console.log('Invalid message');
            break;

      }
    }
    catch (e)
    {
      //console.log(e);
      //console.log(data);
      //console.log(socket);
    }

};

const getUserData = async (socket, Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = await userData.createUserId();
    if(user.username == '') user.username = await userData.createUsername();

    console.log('Adding new client to dictionary');
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

};
