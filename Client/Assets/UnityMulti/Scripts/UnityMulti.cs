using System;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections;

public class UnityMulti : MonoBehaviour
{
    protected ConnectionHandler connection;
    protected User userData;

    public WebSocketState connectionState;
    public bool AutoReconnect { get; set; } = true;

    private string connectionURL;
    private bool wasConnected = false;

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

    public void CreateConnection()
    {
        connection = new ConnectionHandler();

        connection.OnConnected += OnConnected;
        connection.OnMessageReceived += OnMessage;
        connection.OnError += OnError;
        connection.OnDisconnected += OnDisconnected;
        connection.OnStateChanged += OnStateChanged;
        Debug.Log(connectionURL);
        connection.Connect(connectionURL);
        wasConnected = true;
    }

    private Coroutine _reconnect = null;
    private bool isReconnecting = false;
    private int reconnectAttempts = 0;

    private const int maxReconnectAttempts = 10;
    private const float reconnectDelay = 5f;

    public IEnumerator Reconnect()
    {
        isReconnecting = true;

        while(reconnectAttempts < maxReconnectAttempts && !connection.IsConnected && isReconnecting)
        {
            Debug.Log("Reconnect attempts " + reconnectAttempts + 1 + "/" + maxReconnectAttempts);
            CreateConnection();

            yield return new WaitForSeconds(reconnectDelay);

            reconnectAttempts++;
        }

        isReconnecting = false;
        if(reconnectAttempts >= maxReconnectAttempts)
        {
            Debug.Log("Max reconnect attempts reached. ");
        }
    }

    public void Disconnect()
    {
        connection.Dispose();
    }

    private void Update()
    {
        if (!isReconnecting && connection.ws.ReadyState == WebSocketState.Closed)
        {
            if (AutoReconnect && _reconnect == null)
            {
                if (wasConnected)
                {
                    _reconnect = StartCoroutine(Reconnect());
                }
            }
        }
    }

    public new void SendMessage(string message)
    {
        connection.SendMessage(message);
    }

    //getting some error from server message
    public virtual void OnMessage(string rec_message)
    {
        try {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(rec_message);
            Debug.Log(serverMessage.Type);
            switch (serverMessage.Type)
            {
                case MessageType.REQUEST_USER_DATA:
                    GetUserData();
                    break;
                case MessageType.USER_DATA:
                    SetUserData(rec_message);
                    break;
                default:
                    Debug.LogWarning("Unknown message type received: " + serverMessage.Type.ToString());
                    break;
            }
        } catch (Exception e)
        {
            Debug.LogError("Recevied message error: " + e.Message + " \nMessage from server: " + rec_message);
        }
        
    }
    public virtual void OnConnected()
    {
        Debug.Log("Connected to server.");
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

    public virtual void OnDestroy()
    {
        if (connection != null)
        {
            connection.Dispose();
        }
    }

    private void GetUserData()
    {
        Message getData = new Message(MessageType.GET_USER_DATA);

        getData.Content = JsonConvert.SerializeObject(userData);

        SendMessage(JsonConvert.SerializeObject(getData));
    }

    private void SetUserData(string rec_message)
    {
        Message data = JsonConvert.DeserializeObject<Message>(rec_message);
        User serverUserData = JsonConvert.DeserializeObject<User>(data.Content);

        if (userData.userId != serverUserData.userId) userData.SetUserId(serverUserData.userId);
        if (userData.username != serverUserData.username) userData.SetUserName(serverUserData.username);
    }
}