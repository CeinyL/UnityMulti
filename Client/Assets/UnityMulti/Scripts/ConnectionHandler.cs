using System;
using System.Collections;
using UnityEngine;
using WebSocketSharp;

public class ConnectionHandler : MonoBehaviour, IDisposable
{
    public WebSocket ws;

    public Action OnConnected;
    public Action<string> OnMessageReceived;
    public Action<string> OnError;
    public Action OnDisconnected;
    public Action OnStateChanged;

    public bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;

    public bool _autoReconnect;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 5f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    public void Connect(string url)
    {
        ws = new WebSocket(url);

        ws.OnOpen += (sender, args) =>
        {
            OnStateChanged?.Invoke();
            OnConnected?.Invoke();
        };

        ws.OnMessage += (sender, message) =>
        {
            OnMessageReceived?.Invoke(message.Data);
        };

        ws.OnError += (sender, error) =>
        {
            OnStateChanged?.Invoke();
            OnError?.Invoke(error.Message);
        };

        ws.OnClose += (sender, close) =>
        {
            OnStateChanged?.Invoke();
            OnDisconnected?.Invoke();

            if(!_isReconnecting && close.Code != 1000)
            {
                if (_autoReconnect)
                {
                    _isReconnecting = true;
                    StartCoroutine(Reconnect());
                }
            }
        };

        ws.Connect();
    }

    private IEnumerator Reconnect()
    {
        while (_isReconnecting && reconnectAttempt < maxReconnectAttempt)
        {
            Debug.Log("Attempting to reconnect... " + reconnectAttempt + 1 + "/" + maxReconnectAttempt);
            Connect(ws.Url.ToString());

            yield return new WaitForSeconds(ReconnectDelaySeconds);
            reconnectAttempt++;
        }

        if(reconnectAttempt >= maxReconnectAttempt)
        {
            StopReconnecting();
            Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
        }
    }

    public void StopReconnecting()
    {
        _isReconnecting = false;
    }

    public new void SendMessage(string message)
    {
        if (IsConnected)
        {
            ws.Send(message);
        }
    }

    public void Dispose()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {   
            ws.Close(1000, "Intentional disconnect");
            OnStateChanged?.Invoke();
        }
    }

    public WebSocketState getState()
    {
        return ws.ReadyState;
    }
}