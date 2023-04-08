



/// <summary>
/// these messages are used check latency.
/// </summary>
const PING = "ping";
const PONG = "pong";
/// <summary>
/// these message types could be used to establisz connection between client and server.
/// </summary>
const CONNECT = "connect";
const DISCONNECT = "disconnect";
/// <summary>
/// these message types could be used to request/send user data from/to server.
/// </summary>
const REQVALIDATION = "ValidationRequest";
const RESVALIDATION = "ValidationResponse";
const USER_DATA_REQUEST = "userDataRequest";
const USER_DATA_RESPONSE = "userDataResponse";
/// <summary>
/// something like closed instantions of the game (e.g. dungeons, missions)?
/// </summary>
const GAME_STATE = "gameState";
/// <summary>
/// these message types could be used to send player position, rotation and scale.
/// </summary>
const PLAYER_POSITION = "playerPosition";
const PLAYER_ROTATION = "playerRotation";
const PLAYER_SCALE = "playerScale";
/// <summary>
/// this message type could be used to send information about the server's status (e.g. number of clients connected, server load) to the clients.
/// </summary>
const SERVER_STATUS = "serverStatus";
/// <summary>
/// this message type could be used to implement chat.
/// </summary>
const CHAT_MESSAGE = "chatMessage";
/// <summary>
/// this message type could be used to send server related information to clients (e.g. server maintenance, scheduled downtime, etc.).
/// </summary>
const SERVER_MESSAGE = "serverMessage";
/// <summary>
/// this message type could be used to make custom message types that aren't included as base message types.
/// </summary> 
let CUSTOM = [];

module.exports = {
    RESVALIDATION,
    REQVALIDATION,
    PING,
    PONG,
    CONNECT,
    DISCONNECT,
    USER_DATA_REQUEST,
    USER_DATA_RESPONSE,
    GAME_STATE,
    PLAYER_POSITION,
    PLAYER_ROTATION,
    PLAYER_SCALE,
    SERVER_STATUS,
    CHAT_MESSAGE,
    SERVER_MESSAGE,
    CUSTOM
  };