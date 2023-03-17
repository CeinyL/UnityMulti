using System;
using UnityEngine;

public class UnityMulti : MonoBehaviour
{
    //tworzenie instancji polaczenia
    protected ConnectionHandler connection;
    protected User userData;

    public void Connect(string url)
    {
        connection = new ConnectionHandler();

        connection.OnConnected += OnConnected;
        connection.OnMessageReceived += OnMessage;
        connection.OnError += OnError;
        connection.OnDisconnected += OnDisconnected;

        connection.Connect(url);
    }

    public new void SendMessage(string message)
    {
        connection.SendMessage(message);
    }
    //getData send only {} instead of message;
    protected virtual void OnConnected()
    {
        Debug.Log("Connected to server.");
        Message getData = new Message(MessageType.GET_USER_DATA, "");
        SendMessage(JsonUtility.ToJson(getData));
    }
    //TODO;
    protected virtual void OnMessage(string rec_message)
    {
        try {
            Message serverMessage = JsonUtility.FromJson<Message>(rec_message);
            switch (serverMessage.Type.ToString())
            {
                case MessageType.USER_DATA:
                    Debug.Log("Received user data: " + serverMessage.Content);
                    break;
                default:
                    Debug.LogWarning("Unknown message type received: " + serverMessage.Type.ToString());
                    break;
            }
        } catch (Exception e)
        {
            Debug.LogError("Recevied message error: " + e.Message);
        }
        
    }

    protected virtual void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }

    protected virtual void OnDisconnected()
    {
        Debug.Log("Disconnected from server.");
    }

    protected virtual void OnDestroy()
    {
        if (connection != null)
        {
            connection.Dispose();
        }
    }

    public void InstantiateUserObject(string prefabName, Vector3 position)
    {

    }
}