using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : UnityMultiNetworkingCallbacks
{
    public string url = "ws://localhost:8080";
    public long ms;
    void Start()
    {
        multiNetworking.Connect(url, "betek");
    }

    // Update is called once per frame
    void Update()
    {
        //this.ms = multiNetworking.GetLatency();
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
    }
}
