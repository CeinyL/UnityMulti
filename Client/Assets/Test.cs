using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{

    public string url = "ws://localhost:8080";
    public User player;

    void Start()
    {
        Connect(url);
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("I override the onconnected but i also use the base");
    }

}
