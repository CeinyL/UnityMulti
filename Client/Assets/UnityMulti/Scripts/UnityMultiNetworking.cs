using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEditor;
using Newtonsoft.Json;

public class UnityMultiNetworking : MonoBehaviour
{
    public ConnectionManager connection { get; set; } = ConnectionManager.Instance;
    private static UnityMultiNetworking _instance;

    public User userData { get; private set; }

    public string connectionURL { get; private set; }
    private WebSocketState connectionState;
    private long pingTimestamp;
    public long latency;
    [HideInInspector]
    public bool isConnectionReady { get; private set; } = false;
    private bool isDisconnecting = false;

    public delegate void ServerMessageHandler(string message);
    public event ServerMessageHandler OnServerMessage;

    public delegate void ErrorHandler(string error);
    public event ErrorHandler OnClientError;

    public delegate void ConnectedHandler();
    public event ConnectedHandler OnClientConnected;

    public delegate void DisconnectedHandler();
    public event DisconnectedHandler OnClientDisconnected;

    public delegate void ConnectionStateHandler(WebSocketState connectionState);
    public event ConnectionStateHandler OnConnectionStateChange;

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
            GameObject obj = new GameObject("ConnectionManager");
            Instance = obj.AddComponent<UnityMultiNetworking>();
        }
        return Instance;
    }

    public void Connect(string url)
    {
        connectionURL = url;
        userData = new User();
        CreateConnection();
    }

    public void Connect(string url, User userData)
    {
        connectionURL = url;
        this.userData = userData;
        CreateConnection();
    }

    private void CreateConnection()
    {
        connection.OnConnected += OnConnected;
        connection.OnMessageReceived += OnMessage;
        connection.OnError += OnError;
        connection.OnDisconnected += OnDisconnected;
        connection.OnStateChanged += OnStateChanged;
        Debug.Log("Connecting to server: "+connectionURL);
        connection.Connect(connectionURL);
    }

    public void Disconnect()
    {
        connection.Dispose();
    }

    public new void SendMessage(string message)
    {
        connection.SendMessage(message);
    }

    public virtual void OnMessage(string rec_message)
    {
        OnServerMessage?.Invoke(rec_message);
    }

    public virtual void OnConnected()
    {
        isConnectionReady = true;
        OnClientConnected?.Invoke();
        InvokeRepeating("SendPing", 1f, 1f);
    }

    public virtual void OnError(string error)
    {
        OnClientError?.Invoke(error);
    }

    public virtual void OnDisconnected()
    {
        OnClientDisconnected?.Invoke();
    }

    public virtual void OnStateChanged()
    {
        connectionState = connection.getState();
        OnConnectionStateChange?.Invoke(connectionState);
    }

    private void OnApplicationQuit()
    {
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            Disconnect();
        }
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

    private void SendPing()
    {
        if (connection.IsConnected && isConnectionReady)
        {
            Message pingMessage = new Message(MessageType.PING, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            connection.SendMessage(JsonConvert.SerializeObject(pingMessage));

            pingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    public void HandlePong(string serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }
}
