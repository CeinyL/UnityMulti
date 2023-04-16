using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UnityMultiNetworking : BaseSingleton<UnityMultiNetworking>, IDisposable
{
    protected override string GetSingletonName()
    {
        return "UnityMultiNetworking";
    }

    private WebSocket ws;
    public UnityMultiUser clientData { get; private set; }

    public bool IsConnected
    {
        get { return ws != null && ws.ReadyState == WebSocketState.Open; }
        private set { }
    }

    public string connectionURL { get; private set; }
    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    private long pingTimestamp;
    private long latency;
    public bool isConnectionReady { get; private set; } = false;
    public bool isDisconnecting { get; private set; } = false;
    public bool isValidated { get; private set; } = false;

    public delegate void ServerMessageEvent(Message serverMessage);
    public event ServerMessageEvent CustomMessage;

    public delegate void ErrorEvent(ErrorEventArgs error);
    public event ErrorEvent ClientError;

    public delegate void ConnectedEvent();
    public event ConnectedEvent ClientConnected;

    public delegate void DisconnectedEvent();
    public event DisconnectedEvent ClientDisconnected;

    public delegate void ConnectionStateEvent();
    public event ConnectionStateEvent ConnectionStateChange;

    public delegate void InitialConnectionEvent();
    public event InitialConnectionEvent InitialConnection;

    public delegate void ValidationSuccessEvent();
    public event ValidationSuccessEvent ValidationSuccess;

    public delegate void ValidationErrorEvent(UnityMultiValidationHelper.ErrorCode errorCode, string ErrorMessage);
    public event ValidationErrorEvent ValidationError;

    private delegate void ReconnectHandler(CloseEventArgs close);
    private event ReconnectHandler ReconnectEvent;

    protected override void Awake()
    {
        base.Awake();
        ReconnectEvent += Reconnect;
    }
    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            Disconnect();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    public void Connect(string url)
    {
        connectionURL = url;
        clientData = new UnityMultiUser();
        CreateConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="username"></param>
    public void Connect(string url, string username)
    {
        connectionURL = url;
        clientData = new UnityMultiUser(username);
        CreateConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateConnection()
    {
        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => InitialConnection?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => RequestValidation());
        };

        ws.OnMessage += (sender, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnServerMessage(message.Data));          
        };

        ws.OnError += (sender, error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => ClientError?.Invoke(error));
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => ClientDisconnected?.Invoke());
            UnityMainThreadDispatcher.Instance().Enqueue(() => ConnectionStateChange?.Invoke());
            isConnectionReady = false;
            isValidated = false;
            ReconnectEvent?.Invoke(close);
        };

        ws.Connect();
    }

    /// <summary>
    /// Auto reconnect method
    /// </summary>
    private void Reconnect(CloseEventArgs close)
    {
        if (!_isReconnecting && close.Code != 1000)
        {
            if (_autoReconnect)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (IsApplicationPlaying())
                    {

                        _isReconnecting = true;

                        while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected)
                        {
                            Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                            Connect(ws.Url.ToString(), clientData.Username);

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
                });
            }
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close(1000, "Intentional disconnect");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Disconnect()
    {
        isDisconnecting = true;
        Dispose();
    }

    /// <summary>
    /// Send any string message to server
    /// </summary>
    /// <param name="message"></param>
    public new void SendMessage(string message)
    {
        if (IsConnected)
        {
            ws.Send(message);
        }
    }

    /// <summary>
    /// Variant of send message which send message to server in form of JSON
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(Message message)
    {
        if (IsConnected)
        {
            ws.Send(JsonConvert.SerializeObject(message));
        }
    }

    /// <summary>
    /// return current websocket state
    /// </summary>
    /// <returns></returns>
    public WebSocketState getState()
    {
        return ws.ReadyState;
    }

    /// <summary>
    /// return latency
    /// </summary>
    /// <returns></returns>
    public long GetLatency()
    {
        if (latency.Equals(null))
            return 0;
        else
            return latency;
    }

    /// <summary>
    /// set timestamp of last ping
    /// </summary>
    /// <param name="pingTimestamp"></param>
    public void setTimeStamp(long pingTimestamp)
    {
        this.pingTimestamp = pingTimestamp;
    }

    /// <summary>
    /// send message with validation request to server
    /// </summary>
    private void RequestValidation()
    {
        Message validationRequest = new Message();
        validationRequest.Type = MessageType.VALIDATION_REQUEST;
        validationRequest.Content = JsonConvert.SerializeObject(clientData);
        validationRequest.Timestamp = GetTimeNow();

        SendMessage(validationRequest);
    }

    /// <summary>
    /// Base server message handler
    /// </summary>
    /// <param name="message"></param>
    private void OnServerMessage(string message)
    {
        try
        {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(message);
            switch (serverMessage.Type)
            {
                case MessageType.PONG:
                    HandlePong(serverMessage.Content);
                    break;
                case MessageType.CONNECT:
                    // handle connect message
                    break;
                case MessageType.DISCONNECT:
                    // handle disconnect message
                    break;
                case MessageType.VALIDATION_RESPONSE:
                    HandleValidation(serverMessage.Content);
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
                        CustomMessage?.Invoke(serverMessage);
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

    /// <summary>
    /// method which set latency based on server message response time
    /// </summary>
    /// <param name="serverMessage"></param>
    private void HandlePong(string serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }

    /// <summary>
    /// validation handler
    /// </summary>
    /// <param name="serverMessage"></param>
    private void HandleValidation(string serverMessage)
    {
        UnityMultiValidationHelper.ValidationResult validationMessage = JsonConvert.DeserializeObject<UnityMultiValidationHelper.ValidationResult>(serverMessage);

        if (validationMessage.Validated)
        {
            isValidated = validationMessage.Validated;
            ValidationSuccess?.Invoke();
            StartCoroutine(SendPing());
        } else
        {
            ValidationError?.Invoke(validationMessage.ErrorCode, UnityMultiValidationHelper.ValidationError(validationMessage));
            Disconnect();
        }
    }

    /// <summary>
    /// Coroutine that send ping to server every second to check response time
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, GetTimeNow().ToString(), GetTimeNow());

            SendMessage(pingMessage);

            setTimeStamp(GetTimeNow());
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    /// <summary>
    /// add event handler script to the object if it doesn't exist yet
    /// </summary>
    public void AddEventHandler()
    {
        if (this.gameObject.GetComponent<UnityMultiEventHandler>() == null)
            this.gameObject.AddComponent<UnityMultiEventHandler>();
    }
    /// <summary>
    /// Method used to get current time
    /// </summary>
    /// <returns>returns time now as long</returns>
    private long GetTimeNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private bool IsApplicationPlaying()
    {
        return Application.isPlaying;
    }

}

