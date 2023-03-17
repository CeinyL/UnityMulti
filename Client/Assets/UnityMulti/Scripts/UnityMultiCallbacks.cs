using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiCallbacks : UnityMulti
{
    public new virtual void OnConnected()
    {
        base.OnConnected();
    }

    public new virtual void OnMessage(string message)
    {
        base.OnMessage(message);
    }

    public new virtual void OnError(string error)
    {
        base.OnError(error);
    }

    public new virtual void OnDisconnected()
    {
        base.OnDisconnected();
    }
}