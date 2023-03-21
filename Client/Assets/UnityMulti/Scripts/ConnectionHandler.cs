using System;
using WebSocketSharp;

public class ConnectionHandler : IDisposable
{
    public WebSocket ws;

    public Action OnConnected;
    public Action<string> OnMessageReceived;
    public Action<string> OnError;
    public Action OnDisconnected;
    public Action OnStateChanged;

    public bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;
    public bool IsConnecting => ws != null && ws.ReadyState == WebSocketState.Connecting;

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
        };

        ws.Connect();
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
        if (ws != null)
        {   
            ws.Close();
            OnStateChanged?.Invoke();
        }
    }

    public WebSocketState getState()
    {
        return ws.ReadyState;
    }
}