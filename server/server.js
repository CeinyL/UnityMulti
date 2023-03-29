const WebSocket = require('ws');
const userData = require('./App/userData');
const messageTypes = require('./App/messageTypes');

let clients = {

};

let message = {
  Type: '',
  Content: ''
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

const HandleMessage = async (socket, message) => { 
  try {
    const serverMessage = JSON.parse(message);
    switch (serverMessage.Type) {
      case messageTypes.PING:
        HandlePing(socket);
        break;
      case messageTypes.CONNECT:
        // handle connect message
        break;
      case messageTypes.DISCONNECT:
        // handle disconnect message
        break;
      case messageTypes.USER_DATA_REQUEST:
        // handle user data request message
        break;
      case messageTypes.USER_DATA_RESPONSE:
        // handle user data response message
        break;
      case messageTypes.GAME_STATE:
        // handle game state message
        break;
      case messageTypes.PLAYER_POSITION:
        // handle player position message
        break;
      case messageTypes.PLAYER_ROTATION:
        // handle player rotation message
        break;
      case messageTypes.PLAYER_SCALE:
        // handle player scale message
        break;
      case messageTypes.SERVER_STATUS:
        // handle server status message
        break;
      case messageTypes.CHAT_MESSAGE:
        // handle chat message
        break;
      case messageTypes.SERVER_MESSAGE:
        // handle server message
        break;
      default:
        if (messageTypes.CUSTOM.includes(serverMessage.type)) {
          // handle custom message type
        } else {
          // unknown message type
        }
        break;
    }
  } catch (e) {
    console.error('Received message error:', e.message, '\nMessage from server:', message);
  }
};

const HandlePing = async (socket) => {
  message = {
    type: messageTypes.PONG,
    timestamp: Date.now(),
  };

  socket.send(JSON.stringify(message));
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
