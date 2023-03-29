using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

public class ConnectionHandler : IDisposable
{
    private static ConnectionHandler _instance;
    private WebSocket ws;

    public Action OnConnected;
    public Action<string> OnMessageReceived;
    public Action<string> OnError;
    public Action OnDisconnected;
    public Action OnStateChanged;

    public bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;

    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public bool _isAppPlaying = false;
    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    private ConnectionHandler()
    {
    }

    public static ConnectionHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConnectionHandler();
            }
            return _instance;
        }
    }

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
            OnError?.Invoke(error.Exception.ToString());
        };

        ws.OnClose += (sender, close) =>
        {
            OnStateChanged?.Invoke();
            OnDisconnected?.Invoke();

            if (!_isReconnecting && close.Code != 1000)
            {
                if (_autoReconnect)
                {
                    _isReconnecting = true;
                    Reconnect();
                }
            }
        };

        ws.Connect();
    }

    private void Reconnect()
    {
        while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected && _isAppPlaying)
        {
            Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
            Connect(ws.Url.ToString());

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

    public void SendMessage(string message)
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