using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UnityMultiNetworking : MonoBehaviour, IDisposable
{
    #region variables
    private static UnityMultiNetworking _instance;

    //private UnityMainThreadDispatcher ThreadDispatcher = UnityMainThreadDispatcher.Instance();
    private WebSocket ws;
    public User clientData { get; private set; }

    public bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;

    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    public static GameObject UnityMultiObject;

    public string connectionURL { get; private set; }
    [HideInInspector]
    public long pingTimestamp;
    private long latency;
    private bool isConnectionReady = false;
    private bool isDisconnecting = false;
    private bool isValidated = false;
    #endregion

    #region events

    public delegate void ServerMessageEvent(Message serverMessage);
    public event ServerMessageEvent CustomMessage;

    public delegate void ErrorEvent(ErrorEventArgs error);
    public event ErrorEvent ClientError;

    public delegate void ConnectedEvent();
    public event ConnectedEvent ClientConnected;

    public delegate void DisconnectedEvent();
    public event DisconnectedEvent ClientDisconnected;

    public delegate void ConnectionStateEvent();
    public event ConnectionStateEvent ConnectionStateChange;

    public delegate void InitialConnectionEvent();
    public event InitialConnectionEvent InitialConnection;

    #endregion

    #region UnityMultiNetworking
    private UnityMultiNetworking()
    {

    }

    public static UnityMultiNetworking Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UnityMultiNetworking();
            }
            return _instance;
        }
        private set { }
    }

    public static UnityMultiNetworking CreateInstance()
    {
        if (Instance == null)
        {
            UnityMultiObject = new GameObject("UnityNetworking");
            Instance = UnityMultiObject.AddComponent<UnityMultiNetworking>();
        }
        return Instance;
    }

    private void OnDisable()
    {
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            Disconnect();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
    }
    #endregion

    #region connection

    public void Connect(string url, string username)
    {
        connectionURL = url;
        clientData = new User(username);
        CreateConnection();
    }

    private void CreateConnection()
    {
        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => InitialConnection?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => RequestValidation());
            //UnityMainThreadDispatcher.Instance().Enqueue(() => StartPinging?.Invoke());
        };

        ws.OnMessage += (sender, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnServerMessage(message.Data));          
        };

        ws.OnError += (sender, error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => ClientError?.Invoke(error));
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => ClientDisconnected?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
            isConnectionReady = false;
            isValidated = false;

            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                if (Application.isPlaying)
                {
                    if (!_isReconnecting && close.Code != 1000)
                    {
                        if (_autoReconnect)
                        {
                            _isReconnecting = true;
                            Reconnect();
                        }
                    }
                }
            });
        };

        ws.Connect();
    }

    public void Reconnect()
    {
        while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected)
        {
            Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
            Connect(ws.Url.ToString(), clientData.username);

            new WaitForSeconds(ReconnectDelaySeconds);
            reconnectAttempt++;
        }

        if (reconnectAttempt >= maxReconnectAttempt)
        {
            Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
        }

        _isReconnecting = false;
        reconnectAttempt = 0;
    }

    public void Dispose()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close(1000, "Intentional disconnect");
        }
    }

    public void Disconnect()
    {
        isDisconnecting = true;
        Dispose();
    }

    #endregion

    #region client_actions

    public new void SendMessage(string message)
    {
        if (IsConnected)
        {
            ws.Send(message);
        }
    }

    public WebSocketState getState()
    {
        return ws.ReadyState;
    }

    public long GetLatency()
    {
        if (latency.Equals(null))
            return 0;
        else
            return latency;
    }

    public IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            SendMessage(JsonConvert.SerializeObject(pingMessage));

            pingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            yield return new WaitForSeconds(1f);
        }
    }

    public void RequestValidation()
    {
        Message validationRequest = new Message();
        validationRequest.Type = MessageType.VALIDATION_REQUEST;
        validationRequest.Content = JsonConvert.SerializeObject(clientData);

        SendMessage(JsonConvert.SerializeObject(validationRequest));
    }

    #endregion

    #region message_handlers

    private void OnServerMessage(string message)
    {
        try
        {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(message);
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
                case MessageType.USER_DATA_RESPONSE:
                    HandleUserData(serverMessage.Content);
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
                        CustomMessage?.Invoke(serverMessage);
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
            Debug.LogError("Recevied message error: " + e.Message + " \nMessage from server: " + message);
        }
    }

    private void HandlePong(string serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }

    private void HandleUserData(string serverMessage)
    {

    }

    private void HandleValidation(string serverMessage)
    {

    }

    #endregion
}