const WebSocket = require('ws');
const messageHandler = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');

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

const db = new DB.database(
  "localhost",
  "root",
  "betolo9528UM",
  "mysql")

const clientsLoop = async () => {
  while(true){
    for (const userId in clients) {
      console.log(""+clients[userId].userId);
      
    }
    //console.log(db.DBSTATE);
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};

process.argv.forEach(function (val, index) {
  if(index==2&&val=="remote");
});


clientsLoop();
//db.Connect();
HandleValidation(null,'{"Type": "ValidationRequest", "Content" : { "username" : "Betek" , "UserID" : "" } }');
//console.log(db.Query("select * from user"));


console.log('Starting WebSocket server: ');


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
      case messageTypes.REQVALIDATION:
        HandleValidation(socket,message);
        break;
      case messageTypes.PING:
        HandlePing(socket,message);
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



