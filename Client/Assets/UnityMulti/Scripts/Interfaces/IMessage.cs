using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{
    /// <summary>
    /// these messages are used check latency.
    /// </summary>
    public const string PING = "ping";
    public const string PONG = "pong";
    /// <summary>
    /// these message types could be used to establisz connection between client and server.
    /// </summary>
    public const string CONNECT = "connect";
    public const string DISCONNECT = "disconnect";
    /// <summary>
    /// these message types could be used to request/send user data from/to server.
    /// </summary>
    public const string USER_DATA_REQUEST = "userDataRequest";
    public const string USER_DATA_RESPONSE = "userDataResponse";
    /// <summary>
    /// something like closed instantions of the game (e.g. dungeons, missions)?
    /// </summary>
    public const string GAME_STATE = "gameState";
    /// <summary>
    /// these message types could be used to send player position, rotation and scale.
    /// </summary>
    public const string PLAYER_POSITION = "playerPosition";
    public const string PLAYER_ROTATION = "playerRotation";
    public const string PLAYER_SCALE = "playerScale";
    /// <summary>
    /// this message type could be used to send information about the server's status (e.g. number of clients connected, server load) to the clients.
    /// </summary>
    public const string SERVER_STATUS = "serverStatus";
    /// <summary>
    /// this message type could be used to implement chat.
    /// </summary>
    public const string CHAT_MESSAGE = "chatMessage";
    /// <summary>
    /// this message type could be used to send server related information to clients (e.g. server maintenance, scheduled downtime, etc.).
    /// </summary>
    public const string SERVER_MESSAGE = "serverMessage";
    /// <summary>
    /// this message type could be used to make custom message types that aren't included as base message types.
    /// </summary> 
    public static List<string> CUSTOM { get; set; }
}

public interface IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }
}

public class Message : IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }

    public Message(string type, string content)
    {
        this.Type = type;
        this.Content = content;
    }
    public Message(string type)
    {
        this.Type = type;
    }

    public Message()
    {

    }

    
}
