using System;
using WebSocketSharp;

public class ConnectionHandler : IDisposable
{
    private WebSocket ws;

    public Action OnConnected;
    public Action<string> OnMessageReceived;
    public Action<string> OnError;
    public Action OnDisconnected;

    public bool IsConnected => ws != null && ws.ReadyState == WebSocketState.Open;

    public void Connect(string url)
    {
        ws = new WebSocket(url);

        ws.OnOpen += (sender, error) =>
        {
            OnConnected?.Invoke();
        };

        ws.OnMessage += (sender, message) =>
        {
            OnMessageReceived?.Invoke(message.Data);
        };

        ws.OnError += (sender, error) =>
        {
            OnError?.Invoke(error.Message);
        };

        ws.OnClose += (sender, close) =>
        {
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
            ws = null;
        }
    }
}