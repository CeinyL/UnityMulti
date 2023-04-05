using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UnityMultiNetworkingCallbacks : MonoBehaviour
{
    [HideInInspector]
    public UnityMultiNetworking multiNetworking;
    
    [HideInInspector]
    public List<string> customMessageTypes { get; private set; } = new List<string>();

    public WebSocketState connectionState;

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.CreateInstance();
        multiNetworking.OnCustomMessage += OnCustomMessage;
        multiNetworking.OnClientError += OnClientError;
        multiNetworking.OnClientConnected += OnClientConnected;
        multiNetworking.OnClientDisconnected += OnClientDisconnected;
        multiNetworking.OnConnectionStateChange += OnConnectionStateChange;
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
        StartCoroutine(Lol());
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

    public IEnumerator Lol()
    {
        Debug.Log("korutynka");
        yield return new WaitForSeconds(1f);
        Debug.Log("korutynka po sekundzie");
    }
}
