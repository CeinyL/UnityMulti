using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UnityMultiNetworkingCallbacks : MonoBehaviour
{
    public UnityMultiNetworking multiNetworking;
    [HideInInspector]
    public List<string> customMessageTypes { get; private set; } = new List<string>();

    public WebSocketState connectionState;

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.Instance;
        multiNetworking.AddEventHandler();
        multiNetworking.CustomMessage += OnCustomMessage;
        multiNetworking.ClientError += OnClientError;
        multiNetworking.ClientConnected += OnClientConnected;
        multiNetworking.ClientDisconnected += OnClientDisconnected;
        multiNetworking.ConnectionStateChange += OnConnectionStateChange;
        multiNetworking.InitialConnection += OnInitialConnection;
        multiNetworking.ValidationSuccess += OnValidationSuccess;
        multiNetworking.ValidationError += OnValidationError;
    }

    public virtual void OnClientError(ErrorEventArgs error)
    {
        Debug.LogError(
            "Error Exception: " + error.Exception + 
            "\nError Message: " + error.Message
            );
    }

    public virtual void OnClientConnected()
    {
        Debug.Log("Connected to server.");
    }

    public virtual void OnClientDisconnected()
    {
        Debug.Log("Disconnected from server.");
    }

    public virtual void OnCustomMessage(Message serverMessage)
    {
        // Handle custom message type
    }

    public virtual void OnConnectionStateChange()
    {
        connectionState = multiNetworking.getState();
    }

    public virtual void OnInitialConnection()
    {
        Debug.Log("Validating user data");
    }

    public virtual void OnValidationSuccess()
    {
        Debug.Log("Successful validation");
    }

    public virtual void OnValidationError(UnityMultiValidationHelper.ErrorCode errorCode, string ErrorMessage)
    {
        Debug.Log("Validation error: \nErrorCode: " + errorCode + "\nErrorMessage: " + ErrorMessage);
    }
}
