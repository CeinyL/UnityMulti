using System;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class UnityMulti : MonoBehaviour
{
    /// <summary>
    /// instance of connection handler allowing every script inheriting from unitymulticallbacks to use the connection functions etc. 
    /// </summary>
    protected ConnectionHandler connection;
    /// <summary>
    /// string with url to server.
    /// </summary>
    private string connectionURL;
    /// <summary>
    /// instance of user data allowing every script inheriting from unitymulticallbacks to use the user data functions etc. 
    /// </summary>
    protected User userData;

    /// <summary>
    /// current connection state.
    /// </summary>
    public WebSocketState connectionState;
    /// <summary>
    /// calculated latency [ms].
    /// </summary>
    public long latency;
    /// <summary>
    /// timestamp of last ping for latency checking.
    /// </summary>
    private long pingTimestamp;
    /// <summary>
    /// set to true after we recive user data.
    /// </summary>
    public bool isConnectionReady { get; set; } = false;
    /// <summary>
    /// list of custom message types.
    /// </summary>
    private List<string> customMessageTypes = new List<string>();

    /// <summary>
    /// connection to server without previously set user data.
    /// </summary>
    /// <param name="url"></param>
    public void Connect(string url)
    {
        connectionURL = url;
        userData = new User();
        CreateConnection();
    }
    /// <summary>
    /// connection to server with set user data e.g. username.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="userData"></param>
    public void Connect(string url, User userData)
    {
        connectionURL = url;
        this.userData = userData;
        CreateConnection();
    }

    public void CreateConnection()
    {
        connection = ConnectionHandler.Instance;

        connection.OnConnected += OnConnected;
        connection.OnMessageReceived += OnMessage;
        connection.OnError += OnError;
        connection.OnDisconnected += OnDisconnected;
        connection.OnStateChanged += OnStateChanged;
        Debug.Log(connectionURL);
        connection.Connect(connectionURL);
    }

    private void Update()
    {
        if (EditorApplication.isPlaying){
            connection._isAppPlaying = true;
        }
    }

    public void Disconnect()
    {
        connection.Dispose();
    }

    public new void SendMessage(string message)
    {
        connection.SendMessage(message);
    }
    /// <summary>
    /// base message handler.
    /// custom type messages are handled in HandleCustomMessage method.
    /// </summary>
    /// <param name="rec_message"></param>
    public virtual void OnMessage(string rec_message)
    {
        //Debug.Log("Recevied message from server: " + rec_message);
        try {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(rec_message);
            switch (serverMessage.Type)
            {
                case MessageType.PONG:
                    HandlePong(serverMessage.Content);
                    break;
                case MessageType.CONNECT:
                    // handle connect message
                    break;
                case MessageType.DISCONNECT:
                    // handle disconnect message
                    break;
                case MessageType.USER_DATA_REQUEST:
                    // handle user data request message
                    break;
                case MessageType.USER_DATA_RESPONSE:
                    // handle user data response message
                    break;
                case MessageType.GAME_STATE:
                    // handle game state message
                    break;
                case MessageType.PLAYER_POSITION:
                    // handle player position message
                    break;
                case MessageType.PLAYER_ROTATION:
                    // handle player rotation message
                    break;
                case MessageType.PLAYER_SCALE:
                    // handle player scale message
                    break;
                case MessageType.SERVER_STATUS:
                    // handle server status message
                    break;
                case MessageType.CHAT_MESSAGE:
                    // handle chat message
                    break;
                case MessageType.SERVER_MESSAGE:
                    // handle server message
                    break;
                default:
                    if (MessageType.CUSTOM.Contains(serverMessage.Type))
                    {
                        HandleCustomMessage(serverMessage);
                    }
                    else
                    {
                        Debug.Log("Unknown message type: " + serverMessage.Type);
                    }
                    break;
            }
        } 
        catch (Exception e)
        {
            Debug.LogError("Recevied message error: " + e.Message + " \nMessage from server: " + rec_message);
        }
        
    }
    /// <summary>
    /// method to handle custom message types
    /// </summary>
    /// <param name="message"></param>
    public virtual void HandleCustomMessage(Message message)
    {
        // Handle custom message type
    }

    public virtual void OnConnected()
    {
        Debug.Log("Connected to server.");
        isConnectionReady = true;
        InvokeRepeating("SendPing", 1f, 1f);
    }

    public virtual void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }

    public virtual void OnDisconnected()
    {
        Debug.Log("Disconnected from server.");
    }

    public virtual void OnStateChanged()
    {
        connectionState = connection.getState();
    }
    /// <summary>
    /// destroying object will cause disconnect
    /// </summary>
    public virtual void OnDestroy()
    {
        if (connection != null)
        {
            Disconnect();
        }
        connection._isAppPlaying = false;
    }

    private void RequestUserData()
    {
        Message getData = new Message(MessageType.USER_DATA_REQUEST);

        getData.Content = JsonConvert.SerializeObject(userData);

        Debug.Log("Sending data" + JsonConvert.SerializeObject(getData));
        SendMessage(JsonConvert.SerializeObject(getData));
    }

    private void UserDataRespond(string rec_message)
    {
        Message data = JsonConvert.DeserializeObject<Message>(rec_message);
        User serverUserData = JsonConvert.DeserializeObject<User>(data.Content);

        Debug.Log(serverUserData.userId);
        Debug.Log(serverUserData.username);

        if (userData.userId != serverUserData.userId) userData.SetUserId(serverUserData.userId);
        if (userData.username != serverUserData.username) userData.SetUserName(serverUserData.username);

        Debug.Log(userData.userId + "|" + userData.username);
    }

    /// <summary>
    /// method used to
    /// </summary>
    private void SendPing()
    {
        if (connection.IsConnected && isConnectionReady)
        {
            Message pingMessage = new Message(MessageType.PING, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            connection.SendMessage(JsonConvert.SerializeObject(pingMessage));

            pingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    private void HandlePong(string serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
        //Debug.Log($"Ping: {latency} ms");
    }
}