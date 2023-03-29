using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;

public class UnityMultiNetworkingCallbacks : MonoBehaviour
{
    [HideInInspector]
    public UnityMultiNetworking multiNetworking;
    
    [HideInInspector]
    public List<string> customMessageTypes { get; private set; } = new List<string>();

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.CreateInstance();
        multiNetworking.OnServerMessage += OnServerMessage;
        multiNetworking.OnClientError += OnClientError;
        multiNetworking.OnClientConnected += OnClientConnected;
        multiNetworking.OnClientDisconnected += OnClientDisconnected;
        multiNetworking.OnConnectionStateChange += OnConnectionStateChange;
    }

    private void OnServerMessage(string message)
    {
        try
        {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(message);
            switch (serverMessage.Type)
            {
                case MessageType.PONG:
                    multiNetworking.HandlePong(serverMessage.Content);
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
            Debug.LogError("Recevied message error: " + e.Message + " \nMessage from server: " + message);
        }
    }

    public virtual void OnClientError(string error)
    {
        Debug.Log("Error: " + error);
    }

    public virtual void OnClientConnected()
    {
        Debug.Log("Connected to server.");
    }

    public virtual void OnClientDisconnected()
    {
        Debug.Log("Disconnected from server.");
    }

    public virtual void HandleCustomMessage(Message message)
    {
        // Handle custom message type
    }

    public virtual void OnConnectionStateChange(WebSocketState connectionState)
    {

    }

    private void RequestUserData()
    {
        Message getData = new Message(MessageType.USER_DATA_REQUEST);

        getData.Content = JsonConvert.SerializeObject(multiNetworking.userData);

        Debug.Log("Sending data" + JsonConvert.SerializeObject(getData));
        SendMessage(JsonConvert.SerializeObject(getData));
    }

    private void UserDataRespond(string rec_message)
    {
        Message data = JsonConvert.DeserializeObject<Message>(rec_message);
        User serverUserData = JsonConvert.DeserializeObject<User>(data.Content);

        Debug.Log(serverUserData.userId);
        Debug.Log(serverUserData.username);

        if (multiNetworking.userData.userId != serverUserData.userId) multiNetworking.userData.SetUserId(serverUserData.userId);
        if (multiNetworking.userData.username != serverUserData.username) multiNetworking.userData.SetUserName(serverUserData.username);

        Debug.Log(multiNetworking.userData.userId + "|" + multiNetworking.userData.username);
    }

}
