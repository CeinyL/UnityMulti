using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{
    public const string CONNECT = "connect";
    public const string MESSAGE = "message";
    public const string DISCONNECT = "disconnect";
    public const string GET_USER_DATA = "getUserData";
    public const string USER_DATA = "userData";
    //pozwala stworzyc now typy wiadomosci
    public string CUSTOM { get; set; }
}

public interface IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }
}

public class Message : IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }

    public Message(string type, string content)
    {
        this.Type = type;
        this.Content = content;
    }
}
