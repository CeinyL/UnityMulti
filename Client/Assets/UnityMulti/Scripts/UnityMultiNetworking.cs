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
    public User userData { get; private set; }

    private bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;

    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    public string connectionURL { get; private set; }
    private  long pingTimestamp;
    public long latency = 0;
    private bool isConnectionReady = false;
    private bool isDisconnecting = false;

    #endregion

    #region events

    public delegate void ServerMessageHandler(Message serverMessage);
    public event ServerMessageHandler OnCustomMessage;

    public delegate void ErrorHandler(ErrorEventArgs error);
    public event ErrorHandler OnClientError;

    public delegate void ConnectedHandler();
    public event ConnectedHandler OnClientConnected;

    public delegate void DisconnectedHandler();
    public event DisconnectedHandler OnClientDisconnected;

    public delegate void ConnectionStateHandler();
    public event ConnectionStateHandler OnConnectionStateChange;

    private delegate void CoroutineStarter();
    private CoroutineStarter StartPinging;
    private CoroutineStarter StartReconnect;
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
            GameObject obj = new GameObject("UnityNetworking");
            Instance = obj.AddComponent<UnityMultiNetworking>();
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
        StartPinging += () => StartCoroutine(SendPing());
        StartReconnect += () => StartCoroutine(Reconnect());
    }

    #endregion

    #region connection

    public void Connect(string url)
    {
        connectionURL = url;
        userData = new User();
        CreateConnection();
    }

    private void CreateConnection()
    {
        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnClientConnected?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnConnectionStateChange?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => RequestValidation());
            UnityMainThreadDispatcher.Instance().Enqueue(() => StartPinging?.Invoke());
            //ThreadDispatcher.Enqueue(() => {
            //    OnClientConnected?.Invoke();
            //    OnConnectionStateChange?.Invoke();
            //    RequestValidation();
            //    SendPing();
            //});
        };

        ws.OnMessage += (sender, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnServerMessage(message.Data));
            //ThreadDispatcher.Enqueue(() => {
            //    OnServerMessage(message.Data);
            //});
            
        };

        ws.OnError += (sender, error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnClientError?.Invoke(error));
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnConnectionStateChange?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnClientError?.Invoke(error);
                OnConnectionStateChange?.Invoke();
            });
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnClientDisconnected?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnConnectionStateChange?.Invoke());
            isConnectionReady = false;
            
                if (!_isReconnecting && close.Code != 1000)
                {
                    if (_autoReconnect)
                    {
                        _isReconnecting = true;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => StartReconnect?.Invoke());
                    }
                }
        };

        ws.Connect();
    }

    private IEnumerator Reconnect()
    {
        if (Application.isPlaying)
        {
            while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected)
            {
                Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                Connect(ws.Url.ToString());

                yield return new WaitForSeconds(ReconnectDelaySeconds);
                reconnectAttempt++;
            }

            if (reconnectAttempt >= maxReconnectAttempt)
            {
                Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
            }

            _isReconnecting = false;
            reconnectAttempt = 0;
        }
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

    private IEnumerator SendPing()
    {
        Debug.Log("Ping outside");
        while (IsConnected)
        {
            Debug.Log("Ping");
            Message pingMessage = new Message(MessageType.PING, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            SendMessage(JsonConvert.SerializeObject(pingMessage));

            pingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            yield return new WaitForSeconds(1f);
        }
    }

    public void RequestValidation()
    {
        Debug.Log("Requesting validation from server");
        SendMessage("Requesting validation from server");
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
                        OnCustomMessage?.Invoke(serverMessage);
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

    private void HandleValidation(string serverMessage)
    {

    }

    #endregion
}
