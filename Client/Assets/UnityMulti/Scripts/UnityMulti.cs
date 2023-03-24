using System;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections;
using UnityEditor;

public class UnityMulti : MonoBehaviour
{
    protected ConnectionHandler connection;
    protected User userData;

    public WebSocketState connectionState;

    private string connectionURL;

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

    public virtual void OnMessage(string rec_message)
    {
        try {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(rec_message);
            Debug.Log(serverMessage.Type);
            switch (serverMessage.Type)
            {
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
        GetUserData();
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
        connection._isAppPlaying = false;
    }

    private void GetUserData()
    {
        Message getData = new Message(MessageType.GET_USER_DATA);

        getData.Content = JsonConvert.SerializeObject(userData);

        Debug.Log("Sending data" + JsonConvert.SerializeObject(getData));
        SendMessage(JsonConvert.SerializeObject(getData));
    }

    private void SetUserData(string rec_message)
    {
        Message data = JsonConvert.DeserializeObject<Message>(rec_message);
        User serverUserData = JsonConvert.DeserializeObject<User>(data.Content);

        Debug.Log(serverUserData.userId);
        Debug.Log(serverUserData.username);

        if (userData.userId != serverUserData.userId) userData.SetUserId(serverUserData.userId);
        if (userData.username != serverUserData.username) userData.SetUserName(serverUserData.username);

        Debug.Log(userData.userId + "|" + userData.username);
    }

    private void OnDisable()
    {
        Disconnect();
    }
}