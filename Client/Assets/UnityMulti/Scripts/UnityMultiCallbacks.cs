using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiCallbacks : UnityMulti
{
    public override void OnConnected()
    {
        base.OnConnected();
    }

    public override void OnMessage(string message)
    {
        base.OnMessage(message);
    }

    public override void OnError(string error)
    {
        base.OnError(error);
    }

    public override void OnDisconnected()
    {
        base.OnDisconnected();
    }

    public override void OnStateChanged()
    {
        base.OnStateChanged();
    }

}